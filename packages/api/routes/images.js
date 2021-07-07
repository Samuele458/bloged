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

const { BadRequestError, NotFoundError } = require("../utils/errors");

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

//get all images in a blog
router.get(
  "/:blogUrlName/images/",
  blogMiddlewares.checkBlogExists("blogUrlName", "params"),
  (req, res, next) => {
    Image.find({ blog: req.checked.blog._id })
      .sort({ updatedOn: -1 })
      .exec((err, data) => {
        if (err) return next(err);

        let imagesData = data.map((img) => {
          return {
            title: img.title,
            description: img.description,
            url: `/blogs/${req.params.blogUrlName}/images/${img.urlName}`,
          };
        });

        res.status(200).json({ success: true, data: imagesData });
      });
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
          msg: `/blogs/${req.params.blogUrlName}/images/${urlName}`,
        });

        //uploading to AWS s3
        const result = await s3Utils.uploadToS3(file);
      } else {
        return next(new Error("Error"));
      }
    });
  }
);

//edit image info
router.put(
  "/:blogUrlName/images/:imageUrlName",
  passport.authenticate("jwt", { session: false }), //jwt auth
  blogMiddlewares.checkBlogExists("blogUrlName", "params"), //check blog
  blogMiddlewares.userIsAdmin, //check if user is admin in blog
  blogMiddlewares.checkImageExists("imageUrlName", "params"),
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

    let updateObj = {};

    if (req.body.title) updateObj.title = req.body.title;
    if (req.body.description) updateObj.description = req.body.description;

    Image.findByIdAndUpdate(
      req.checked.images[0]._id,
      updateObj,
      (err, data) => {
        if (err) return next(err);

        console.log("Update:", updateObj, "Data:", data);
        if (data)
          res.status(200).json({ success: true, msg: "Image updated." });
        else next(new NotFoundError("Image not found."));
      }
    );
  }
);

//delete image
router.delete(
  "/:blogUrlName/images/:imageUrlName",
  passport.authenticate("jwt", { session: false }), //jwt auth
  blogMiddlewares.checkBlogExists("blogUrlName", "params"), //check blog
  blogMiddlewares.userIsAdmin, //check if user is admin in blog
  blogMiddlewares.checkImageExists("imageUrlName", "params"),
  async (req, res, next) => {
    console.log(req.checked.images[0].key);
    try {
      const result = await s3Utils.deleteFromS3(req.checked.images[0].key);
      console.log(result);
    } catch (err) {
      return next(err);
    }

    Image.findByIdAndDelete(req.checked.images[0]._id, (err, data) => {
      if (err) return next(err);

      if (data)
        res
          .status(200)
          .json({ success: true, msg: "Image succesfully deleted." });
      else next(new NotFoundError("Image not found."));
    });
  }
);

module.exports = router;
