const mongoose = require("mongoose");
const router = require("express").Router();
const passport = require("passport");
const authUtils = require("../utils/auth");
const blogMiddlewares = require("./blogMiddlewares");
const Tag = mongoose.model("Tag");
const Blog = mongoose.model("Blog");
const { checkField } = require("../utils/generic");
const multer = require("multer");
const upload = multer({
  dest: "uploads/",
  limits: { fileSize: 1 * 1024 * 1024 },
});

router.post(
  "/:blogUrlName/images/",
  upload.single("image"),
  blogMiddlewares.checkBlogExists("blogUrlName", "params"), //TODO check user right in blog
  (req, res, next) => {
    return next(Error("sdfdfsdfsdf"));

    //checking title
    if (
      checkField(
        req,
        "title",
        "body",
        (value) => typeof value === "string" || typeof value === "undefined"
      ) === null
    )
      return res.status(400).json({ success: false, msg: "Invalid title" });

    //checking description
    if (
      checkField(
        req,
        "description",
        "body",
        (value) => typeof value === "string" || typeof value === "undefined"
      ) === null
    )
      return res
        .status(400)
        .json({ success: false, msg: "Invalid description" });

    const file = req.file;
    console.log(file);

    res.send("Done");
  }
);

module.exports = router;
