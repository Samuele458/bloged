const mongoose = require("mongoose");
const router = require("express").Router();
const passport = require("passport");
const authUtils = require("../utils/auth");
const { checkField } = require("../utils/generic");
const blogMiddlewares = require("./blogMiddlewares");
const Tag = mongoose.model("Tag");
const Post = mongoose.model("Post");
const Blog = mongoose.model("Blog");

router.get(
  "/:blogUrlName/posts/:postUrlName",
  blogMiddlewares.checkBlogExists("blogUrlName", "params"),
  blogMiddlewares.checkPostExists("postUrlName", "params"),
  (req, res, next) => {
    let post = req.checked.posts[0];

    res.status(200).json({
      success: true,
      data: {
        urlName: post.urlName,
        fullName: post.fullName,
        text: post.text,
      },
    });
  }
);

router.post(
  "/:blogUrlName/posts",
  passport.authenticate("jwt", { session: false }), //jwt auth
  blogMiddlewares.checkBlogExists("blogUrlName", "params"), //check blog
  blogMiddlewares.userIsAdmin, //check if user is admin in blog
  blogMiddlewares.checkTagExists("tags", "body"), //check blog tags
  (req, res, next) => {
    let text = req.body.text;
    let tags = req.body.tags;

    //checking formats
    let urlName = checkField(req, "urlName", "body", "urlName");
    if (urlName === null)
      return next(new BadRequestError("Invalid url name format."));

    let fullName = checkField(req, "fullName", "body", "fullName");
    if (fullName === null)
      return next(new BadRequestError("Invalid full name format."));

    //text is not necessary
    if (checkField(req, "text", "body") !== null) {
      text = checkField(req, "text", "body", "text");
      if (text === null)
        return next(new BadRequestError("Invalid text format."));
    }

    if (!(Array.isArray(tags) || typeof tags === "undefined")) {
      return next(new BadRequestError("Invalid tags format."));
    }

    Post.findOne(
      {
        urlName: urlName,
        blog: mongoose.Types.ObjectId(req.checked.blog._id),
      },
      (err, postFound) => {
        if (err) return next(err);

        if (postFound) new BadRequestError("Post already exists.");

        const newPost = new Post({
          urlName: urlName,
          fullName: fullName,
          text: text,
          blog: mongoose.Types.ObjectId(req.checked.blog._id),
          tags: req.checked.tags.map((tag) => mongoose.Types.ObjectId(tag._id)),
        });

        newPost.save((err, data) => {
          if (err) return next(err);

          res.status(200).json({ success: true });
        });
      }
    );
  }
);

router.put(
  "/:blogUrlName/posts/:postUrlName",
  passport.authenticate("jwt", { session: false }), //jwt auth
  blogMiddlewares.checkBlogExists("blogUrlName", "params"), //check blog
  blogMiddlewares.userIsAdmin, //check if user is admin in blog
  blogMiddlewares.checkPostExists("postUrlName", "params"), //check post
  (req, res, next) => {
    let urlName;
    let fullName;
    let text;

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

    if (checkField(req, "text", "body") !== null) {
      text = checkField(req, "text", "body", "text");
      if (text === null)
        return next(new BadRequestError("Invalid full name format."));
      queryObj.text = text;
    }

    if (typeof tags !== "undefined")
      if (!Array.isArray(tags)) {
        return next(new BadRequestError("Invalid tags format."));
      } else queryObj.tags = req.checked.tags;

    //checks if user requested to change UrlName
    if (queryObj.hasOwnProperty("urlName")) {
      //checks if the new urlName exists (it must be unique)
      Post.findOne({ urlName: queryObj.urlName }, (err, postFound) => {
        if (err) return next(err);

        if (postFound) return next(new BadRequestError("Post already exists."));
        //urlName is unique. post can be edited
        else
          Post.findByIdAndUpdate(
            req.checked.posts[0]._id,
            queryObj,
            (err, editedPost) => {
              if (err) return next(err);

              if (editedPost) res.json({ success: true });
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
  "/:blogUrlName/posts/:postUrlName",
  passport.authenticate("jwt", { session: false }), //jwt auth
  blogMiddlewares.checkBlogExists("blogUrlName", "params"), //check blog
  blogMiddlewares.userIsAdmin, //check if user is admin in blog
  blogMiddlewares.checkPostExists("postUrlName", "params"), //check post
  (req, res, next) => {
    let post = req.checked.posts[0];

    Post.findByIdAndDelete(post._id, (err, postDeleted) => {
      if (err) return next(err);

      if (postDeleted) res.status(200).json({ success: true });
      else next(new Error("COuld not delete post."));
    });
  }
);

module.exports = router;
