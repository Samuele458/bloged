const mongoose = require("mongoose");
const router = require("express").Router();
const passport = require("passport");
const authUtils = require("../lib/authUtils");
const Tag = mongoose.model("Tag");
const Blog = mongoose.model("Blog");

router.get("/:blogUrlName/tags/:tagUrlName", (req, res, next) => {
  Blog.findOne({ urlName: req.params.blogUrlName }, (err, blog) => {
    if (err) next(err);

    //blog doesn't exist
    if (!blog)
      return res.status(404).json({ success: false, msg: "Unknown blog." });
    else
      Tag.findOne(
        {
          urlName: req.params.tagUrlName,
          blog: mongoose.Types.ObjectId(blog._id),
        },
        (err, tag) => {
          if (err) next(err);

          if (tag)
            res.status(200).json({
              success: true,
              data: {
                urlName: tag.urlName,
                fullName: tag.fullName,
                description: tag.description,
              },
            });
          else res.status(404).json({ succe: false, msg: "Unknown tag." });
        }
      );
  });
});

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

//edit blog
router.put(
  "/:blogUrlName/tags/:tagUrlName",
  passport.authenticate("jwt", { session: false }),
  (req, res, next) => {
    let urlName = req.body.urlName;
    let fullName = req.body.fullName;
    let description = req.body.description;

    let queryObj = {};

    //verifying tag urlname
    if (urlName)
      if (!authUtils.validateUrlnameFormat(urlName.trim()))
        return res
          .status(400)
          .json({ success: false, msg: "Invalid url name format" });
      else queryObj.urlName = urlName.trim();

    //verifying tag fullName
    if (fullName) queryObj.fullName = fullName.trim();

    //verifying description
    if (description) queryObj.description = description;

    //checks if the blog in the params field actually exists
    Blog.findOne({ urlName: req.params.blogUrlName }, (err, blog) => {
      if (err) next(err);

      //blog doesn't exist
      if (!blog)
        return res.status(404).json({ success: false, msg: "Unknown blog." });

      //checks if user has rights to edit blog
      if (!blog.admins.includes(req.user._id))
        return res.status(401).json({ success: false, msg: "Unauthorized." });

      Tag.findOne(
        { urlName: req.params.tagUrlName, blog: blog._id },
        (err, tagToEdit) => {
          if (err) next(err);

          if (!tagToEdit)
            return res
              .status(404)
              .json({ success: false, msg: "Unknown tag." });

          //checks if user requested to change UrlName
          if (queryObj.hasOwnProperty("urlName")) {
            //checks if the new urlName exists (it must be unique)
            Tag.findOne({ urlName: queryObj.urlName }, (err, tagFound) => {
              if (tagFound)
                res.status(400).json({
                  success: false,
                  msg: "New url name tag already exists.",
                });
              //urlName is unique. Tag can be edited
              else
                Tag.findByIdAndUpdate(
                  tagToEdit._id,
                  queryObj,
                  (err, editedTag) => {
                    if (err) next(err);

                    if (editedTag) res.json({ success: true });
                    else
                      return res.json({
                        success: false,
                        msg: "Could not edit tag",
                      });
                  }
                );
            });
          } else
            Tag.findByIdAndUpdate(tagToEdit._id, queryObj, (err, editedTag) => {
              if (err) next(err);

              if (editedTag) res.json({ success: true });
              else res.json({ success: false, msg: "Could not edit tag" });
            });
        }
      );
    });
  }
);

router.delete(
  "/:blogUrlName/tags/:tagUrlName",
  passport.authenticate("jwt", { session: false }),
  (req, res, next) => {
    Blog.findOne({ urlName: req.params.blogUrlName }, (err, blog) => {
      if (err) next(err);

      //blog doesn't exist
      if (!blog)
        return res.status(404).json({ success: false, msg: "Unknown blog." });
      else if (!blog.admins.includes(req.user._id))
        return res.status(401).json({ success: false, msg: "Unauthorized." });
      else
        Tag.findOneAndDelete(
          { urlName: req.params.tagUrlName },
          (err, tagDeleted) => {
            if (err) next(err);

            if (tagDeleted) res.status(200).json({ success: true });
            else res.status(404).json({ succe: false, msg: "Unknown tag." });
          }
        );
    });
  }
);

module.exports = router;
