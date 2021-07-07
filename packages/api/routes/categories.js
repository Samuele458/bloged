const mongoose = require("mongoose");
const router = require("express").Router();
const passport = require("passport");
const { BadRequestError } = require("../utils/errors");
const { checkField } = require("../utils/generic");
const Category = mongoose.model("Category");
const blogMiddlewares = require("./blogMiddlewares");

router.get(
  "/:blogUrlName/categories/:categoryUrlName",
  blogMiddlewares.checkBlogExists("blogUrlName", "params"),
  blogMiddlewares.checkCategoryExists("categoryUrlName", "params"),
  (req, res, next) => {
    let category = req.checked.categories[0];

    res.status(200).json({
      success: true,
      data: {
        urlName: category.urlName,
        fullName: category.fullName,
        description: category.description,
      },
    });
  }
);

router.post(
  "/:blogUrlName/categories/",
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

    Category.findOne(
      {
        blog: mongoose.Types.ObjectId(req.checked.blog._id),
        urlName: urlName,
      },
      (err, categoryFound) => {
        if (err) return next(err);

        if (categoryFound) next(new BadRequestError("Category already exists"));
        else {
          const newCategory = new Category({
            urlName: urlName,
            fullName: fullName,
            description: description,
            blog: mongoose.Types.ObjectId(req.checked.blog._id),
          });

          newCategory.save((err, data) => {
            if (err) return next(err);

            res.status(200).json({ success: true });
          });
        }
      }
    );
  }
);

router.put(
  "/:blogUrlName/categories/:categoryUrlName",
  passport.authenticate("jwt", { session: false }),
  blogMiddlewares.checkBlogExists("blogUrlName", "params"), //check blog
  blogMiddlewares.userIsAdmin, //check if user is admin in blog
  blogMiddlewares.checkCategoryExists("categoryUrlName", "params"), //check category
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
      Category.findOne({ urlName: queryObj.urlName }, (err, categoryFound) => {
        if (err) return next(err);

        if (categoryFound)
          return next(new BadRequestError("Category already exists."));
        //urlName is unique. Category can be edited
        else
          Category.findByIdAndUpdate(
            req.checked.categories[0]._id,
            queryObj,
            (err, editedCategory) => {
              if (err) return next(err);

              if (editedCategory) res.json({ success: true });
              else return next(new Error("Could not edit category."));
            }
          );
      });
    } else
      Category.findByIdAndUpdate(
        req.checked.categories[0]._id,
        queryObj,
        (err, editedCategory) => {
          if (err) return next(err);

          if (editedCategory) res.json({ success: true });
          else res.json({ success: false, msg: "Could not edit category." });
        }
      );
  }
);

router.delete(
  "/:blogUrlName/categories/:categoryUrlName",
  passport.authenticate("jwt", { session: false }), //jwt auth
  blogMiddlewares.checkBlogExists("blogUrlName", "params"), //check blog
  blogMiddlewares.userIsAdmin, //check if user is admin in blog
  blogMiddlewares.checkCategoryExists("categoryUrlName", "params"), //check category
  (req, res, next) => {
    let category = req.checked.categories[0];

    Category.findByIdAndDelete(category._id, (err, categoryDeleted) => {
      if (err) return next(err);

      if (categoryDeleted) res.status(200).json({ success: true });
      else next(new Error("Could not delete category."));
    });
  }
);

module.exports = router;
