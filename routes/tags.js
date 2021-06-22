const mongoose = require("mongoose");
const router = require("express").Router();
const passport = require("passport");
const authUtils = require("../lib/authUtils");
const Tag = mongoose.model("Tag");
const Blog = mongoose.model("Blog");

router.get(
  "/:blogUrlName/tags/:tagUrlName",
  //blogUtils.blogExists("param", tagUrlName),
  (req, res, next) => {}
);

router.post(
  "/:blogUrlName/tags/",
  passport.authenticate("jwt", { session: false }),
  (req, res, next) => {
    let urlName = req.body.urlName;
    let fullName = req.body.fullName;
    let description = req.body.description;

    //checking formats
    if (!(urlName && authUtils.validateUrlnameFormat(urlName.trim())))
      return res
        .status(400)
        .json({ success: false, msg: "Invalid url name format" });
    else if (!fullName)
      return res
        .status(400)
        .json({ success: false, msg: "Invalid full name format" });

    //description is not necessary
    if (description) description = description.trim();

    urlName = urlName.trim();
    fullName = fullName.trim();

    Blog.findOne({ urlName: req.params.blogUrlName }, (err, blog) => {
      if (err) next(err);

      //check if blog exists
      if (!blog) res.status(404).json({ success: false, msg: "Unknown blog." });
      //checks if user has rights to edit blog
      else if (!blog.admins.includes(req.user._id))
        return res.status(401).json({ success: false, msg: "Unauthorized." });
      else
        Tag.findOne(
          {
            blog: mongoose.Types.ObjectId(blog._id),
            urlName: urlName,
          },
          (err, tagFound) => {
            if (err) next(err);

            if (tagFound)
              res.json({ success: false, msg: "Tag already exists." });
            else {
              const newTag = new Tag({
                urlName: urlName,
                fullName: fullName,
                description: description,
                blog: mongoose.Types.ObjectId(blog._id),
              });

              newTag.save((err, data) => {
                if (err) next(err);

                res.status(200).json({ success: true });
              });
            }
          }
        );
    });
  }
);

router.put("/:blogUrlName/tags/:tagUrlName", (req, res, next) => {});

router.delete("/:tagUrlName", (req, res, next) => {});

module.exports = router;
