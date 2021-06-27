const mongoose = require("mongoose");
const router = require("express").Router();
const crypto = require("crypto");
const passport = require("passport");
const blogMiddlewares = require("./blogMiddlewares");
const base64url = require("base64url");
const Image = mongoose.model("Image");
const { checkField } = require("../utils/generic");
const multer = require("multer");
const storage = multer.memoryStorage();
const upload = multer({
  storage: storage,
  limits: { fileSize: 1 * 1024 * 1024 },
  fileFilter: function (req, file, cb) {
    if (
      file.mimetype === "image/png" ||
      file.mimetype === "image/gif" ||
      file.mimetype === "image/jpg" ||
      file.mimetype === "image/jpeg"
    )
      cb(null, true);
    else cb(null, false);
  },
});

const s3Utils = require("../utils/s3");

const { BadRequestError } = require("../utils/errors");

//get an existing image
router.get(
  "/:blogUrlName/images/:imageUrlName",
  blogMiddlewares.checkBlogExists("blogUrlName", "params"),
  blogMiddlewares.checkImageExists("imageUrlName", "params"),
  (req, res, next) => {
    //res.send(req.checked.images[0].key);
    const readStream = s3Utils.downloadFromS3(req.checked.images[0].key);

    readStream.pipe(res);
  }
);

//create new image
router.post(
  "/:blogUrlName/images/",
  upload.single("image"), //get file
  passport.authenticate("jwt", { session: false }), //jwt auth
  blogMiddlewares.checkBlogExists("blogUrlName", "params"), //check blog
  blogMiddlewares.userIsAdmin, //check if user is admin in blog
  (req, res, next) => {
    //checking title
    if (
      checkField(
        req,
        "title",
        "body",
        (value) => typeof value === "string" || typeof value === "undefined"
      ) === null
    )
      return next(new BadRequestError("Invalid title"));

    //checking description
    if (
      checkField(
        req,
        "description",
        "body",
        (value) => typeof value === "string" || typeof value === "undefined"
      ) === null
    )
      return next(new BadRequestError("Invalid description"));

    //checking if file exists
    const file = req.file;
    if (!file) return next(new BadRequestError("Missing or invalid image"));

    //splitting filename and extension
    let filename, extension;
    filename = file.originalname.split(".");
    extension = filename.pop();
    filename = filename.join(".");

    //creating random url name, based on filename, and a small random string, in order to avoid duplicates
    const urlName =
      filename + base64url(crypto.randomBytes(4)) + "." + extension;

    file.filename = base64url(crypto.randomBytes(32));

    const newImage = new Image({
      title: req.body.title,
      description: req.body.description,
      key: file.filename,
      urlName: urlName,
      createdOn: new Date(),
      updatedOn: new Date(),
      blog: mongoose.Types.ObjectId(req.checked.blog._id),
    });

    newImage.save(async (err, imageData) => {
      if (err) return next(err);

      if (imageData) {
        res.status(200).json({
          success: true,
          msg: `/images/${req.params.blogUrlName}/images/${urlName}`,
        });

        //uploading to AWS s3
        const result = await s3Utils.uploadToS3(file);
      } else {
        return next(new Error("Error"));
      }
    });
  }
);

//delete image
router.delete(
  "/:blogUrlName/images/:imageUrlName",
  passport.authenticate("jwt", { session: false }), //jwt auth
  blogMiddlewares.checkBlogExists("blogUrlName", "params"), //check blog
  blogMiddlewares.userIsAdmin, //check if user is admin in blog
  blogMiddlewares.checkImageExists("imageUrlName", "params"),
  (req, res, next) => {}
);

module.exports = router;
