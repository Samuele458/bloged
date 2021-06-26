const mongoose = require("mongoose");
const router = require("express").Router();
const passport = require("passport");
const authUtils = require("../lib/authUtils");
const blogMiddlewares = require("./blogMiddlewares");
const Tag = mongoose.model("Tag");
const Blog = mongoose.model("Blog");
const { checkField } = require("../lib/utils");
const multer = require("multer");
const upload = multer({ dest: "uploads/" });

router.post(
  "/:blogUrlName/images/",
  blogMiddlewares.checkBlogExists("blogUrlName", "params"), //TODO check user right in blog
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

    res.send("Done");
  }
);

module.exports = router;
