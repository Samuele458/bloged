const mongoose = require("mongoose");
const router = require("express").Router();
const passport = require("passport");
const authUtils = require("../utils/auth");
const { BadRequestError } = require("../utils/errors");
const { checkField } = require("../utils/generic");
const Tag = mongoose.model("Tag");
const Blog = mongoose.model("Blog");
const blogMiddlewares = require("./blogMiddlewares");

router.get(
  "/:blogUrlName/tags/:tagUrlName",
  blogMiddlewares.checkBlogExists("blogUrlName", "params"),
  blogMiddlewares.checkTagExists("tagUrlName", "params"),
  (req, res, next) => {
    let tag = req.checked.tags[0];

    res.status(200).json({
      success: true,
      data: {
        urlName: tag.urlName,
        fullName: tag.fullName,
        description: tag.description,
      },
    });
  }
);

router.post(
  "/:blogUrlName/tags/",
  passport.authenticate("jwt", { session: false }),
  blogMiddlewares.checkBlogExists("blogUrlName", "params"),
  blogMiddlewares.userIsAdmin, //check if user is admin in blog
  (req, res, next) => {
    let description;

    //checking formats
    let urlName = checkField(req, "urlName", "body", "urlName");
    if (urlName === null)
      return next(new BadRequestError("Invalid url name format."));

    let fullName = checkField(req, "fullName", "body", "fullName");
    if (fullName === null)
      return next(new BadRequestError("Invalid full name format."));

    //description is not necessary
    if (checkField(req, "description", "body") !== null) {
      description = checkField(req, "description", "body", "text");
      if (description === null)
        return next(new BadRequestError("Invalid description format."));
    }

    Tag.findOne(
      {
        blog: mongoose.Types.ObjectId(req.checked.blog._id),
        urlName: urlName,
      },
      (err, tagFound) => {
        if (err) return next(err);

        if (tagFound) next(new BadRequestError("Tag already exists"));
        else {
          const newTag = new Tag({
            urlName: urlName,
            fullName: fullName,
            description: description,
            blog: mongoose.Types.ObjectId(req.checked.blog._id),
          });

          newTag.save((err, data) => {
            if (err) return next(err);

            res.status(200).json({ success: true });
          });
        }
      }
    );
  }
);

//edit tag
router.put(
  "/:blogUrlName/tags/:tagUrlName",
  passport.authenticate("jwt", { session: false }),
  blogMiddlewares.checkBlogExists("blogUrlName", "params"), //check blog
  blogMiddlewares.userIsAdmin, //check if user is admin in blog
  blogMiddlewares.checkTagExists("tagUrlName", "params"), //check tag
  (req, res, next) => {
    let urlName;
    let fullName;
    let description;

    let queryObj = {};

    if (checkField(req, "urlName", "body") !== null) {
      urlName = checkField(req, "urlName", "body", "urlName");
      if (urlName === null)
        return next(new BadRequestError("Invalid url name format."));
      queryObj.urlName = urlName;
    }

    if (checkField(req, "fullName", "body") !== null) {
      fullName = checkField(req, "fullName", "body", "fullName");
      if (fullName === null)
        return next(new BadRequestError("Invalid full name format."));
      queryObj.fullName = fullName;
    }

    if (checkField(req, "description", "body") !== null) {
      description = checkField(req, "description", "body", "text");
      if (description === null)
        return next(new BadRequestError("Invalid full name format."));
      queryObj.description = description;
    }

    //checks if user requested to change UrlName
    if (queryObj.hasOwnProperty("urlName")) {
      //checks if the new urlName exists (it must be unique)
      Tag.findOne({ urlName: queryObj.urlName }, (err, tagFound) => {
        if (err) return next(err);

        if (tagFound) return next(new BadRequestError("Tag already exists."));
        //urlName is unique. Tag can be edited
        else
          Tag.findByIdAndUpdate(
            req.checked.tags[0]._id,
            queryObj,
            (err, editedTag) => {
              if (err) return next(err);

              if (editedTag) res.json({ success: true });
              else return next(new Error("Could not edit tag."));
            }
          );
      });
    } else
      Tag.findByIdAndUpdate(
        req.checked.tags[0]._id,
        queryObj,
        (err, editedTag) => {
          if (err) return next(err);

          if (editedTag) res.json({ success: true });
          else res.json({ success: false, msg: "Could not edit tag." });
        }
      );
  }
);

router.delete(
  "/:blogUrlName/tags/:tagUrlName",
  passport.authenticate("jwt", { session: false }), //jwt auth
  blogMiddlewares.checkBlogExists("blogUrlName", "params"), //check blog
  blogMiddlewares.userIsAdmin, //check if user is admin in blog
  blogMiddlewares.checkTagExists("tagUrlName", "params"), //check tag
  (req, res, next) => {
    let tag = req.checked.tags[0];

    Tag.findByIdAndDelete(tag._id, (err, tagDeleted) => {
      if (err) return next(err);

      if (tagDeleted) res.status(200).json({ success: true });
      else next(new Error("COuld not delete tag."));
    });
  }
);

module.exports = router;
