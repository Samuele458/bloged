const mongoose = require("mongoose");
const router = require("express").Router();
const passport = require("passport");
const authUtils = require("../utils/auth");
const Blog = mongoose.model("Blog");
const blogMiddlewares = require("./blogMiddlewares");
const { checkField } = require("../utils/generic");
const { BadRequestError } = require("../utils/errors");

//get blog data
router.get(
  "/:blogUrlName",
  blogMiddlewares.checkBlogExists("blogUrlName", "params"),
  (req, res, next) => {
    res.status(200).json({
      success: true,
      data: {
        urlName: req.checked.blog.urlName,
        fullName: req.checked.blog.fullName,
      },
    });
  }
);

//create new blog
router.post(
  "/",
  passport.authenticate("jwt", { session: false }),
  (req, res, next) => {
    let admin = req.user._id;

    let urlName = checkField(req, "urlName", "body", "urlName");
    if (urlName === null)
      return next(new BadRequestError("Invalid url name format."));

    let fullName = checkField(req, "fullName", "body", "fullName");
    if (fullName === null)
      return next(new BadRequestError("Invalid full name format."));

    Blog.findOne({ urlName: urlName }, (err, data) => {
      if (err) return next(err);

      //check if another blog with the same name already exists
      if (data) return next(new BadRequestError("Blog already exists"));

      //creating blog
      const newBlog = new Blog({
        urlName: urlName,
        fullName: fullName,
        admins: [mongoose.Types.ObjectId(admin)],
      });

      newBlog.save((err, data) => {
        if (err) return next(err);
        res.status(200).send({
          success: true,
          data: data,
        });
      });
    });
  }
);

//edit blog
router.put(
  "/:blogUrlName",
  passport.authenticate("jwt", { session: false }), //jwt auth
  blogMiddlewares.checkBlogExists("blogUrlName", "params"), //check blog
  blogMiddlewares.userIsAdmin, //check if user is admin in blog
  (req, res, next) => {
    let urlName;
    let fullName;

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

    //checks if user requested to change UrlName
    if (queryObj.hasOwnProperty("urlName")) {
      //checks if the new urlName exists (it must be unique)
      Blog.findOne({ urlName: queryObj.urlName }, (err, blogFound) => {
        if (err) return next(err);

        if (blogFound)
          next(new BadRequestError("New url name blog already exists."));
        //urlName is unique. Blog can be edited
        else
          Blog.findByIdAndUpdate(
            req.checked.blog._id,
            queryObj,
            (err, editedBlog) => {
              if (err) return next(err);

              if (editedBlog) res.status(200).json({ success: true });
              else next(new Error("Could not edit blog."));
            }
          );
      });
    } else
      Blog.findByIdAndUpdate(
        req.checked.blog._id,
        queryObj,
        (err, editedBlog) => {
          if (err) return next(err);

          if (editedBlog) res.status(200).json({ success: true });
          else next(new Error("Could not edit blog."));
        }
      );
  }
);

//delete blog
router.delete(
  "/:blogUrlName",
  passport.authenticate("jwt", { session: false }), //jwt auth
  blogMiddlewares.checkBlogExists("blogUrlName", "params"), //check blog
  blogMiddlewares.userIsAdmin, //check if user is admin in blog
  (req, res, next) => {
    Blog.findByIdAndDelete(req.checked.blog._id, (err, blogDeleted) => {
      if (err) return next(err);

      res.status(200).json({ success: true });
    });
  }
);

//TODO: add /:blog/admin for add and remove admins from blog

module.exports = router;
