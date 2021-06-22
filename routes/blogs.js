const mongoose = require("mongoose");
const router = require("express").Router();
const passport = require("passport");
const authUtils = require("../lib/authUtils");
const Blog = mongoose.model("Blog");

//get blog data
router.get("/:blogUrlName", (req, res, next) => {
  Blog.findOne({ urlName: req.params.blogUrlName }, (err, blogData) => {
    if (err) next(err);

    if (blogData)
      res.status(200).json({
        success: true,
        data: {
          urlName: blogData.urlName,
          fullName: blogData.fullName,
        },
      });
    else
      res.status(404).json({
        success: false,
        msg: "Unknown blog.",
      });
  });
});

//create new blog
router.post(
  "/",
  passport.authenticate("jwt", { session: false }),
  (req, res) => {
    let urlName = req.body.urlName;
    let fullName = req.body.fullName;
    let admin = req.user._id;

    //checking formats
    if (!(urlName && authUtils.validateUrlnameFormat(urlName.trim())))
      return res.json({ success: false, msg: "Invalid url name format" });
    else if (!fullName)
      return res.json({ success: false, msg: "Invalid full name format" });

    urlName = urlName.trim();
    fullName = fullName.trim();

    Blog.findOne({ urlName: urlName }, (err, data) => {
      if (err) next(err);

      //check if another blog with the same name already exists
      if (data)
        return res.json({ success: false, msg: "Url name already exists." });

      //creating blog
      const newBlog = new Blog({
        urlName: urlName,
        fullName: fullName,
        admins: [mongoose.Types.ObjectId(admin)],
      });

      newBlog.save((err, data) => {
        if (err) next(err);
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
  passport.authenticate("jwt", { session: false }),
  (req, res, next) => {
    let urlName = req.body.urlName;
    let fullName = req.body.fullName;

    let queryObj = {};

    //verifying blog urlname
    if (urlName)
      if (!authUtils.validateUrlnameFormat(urlName.trim()))
        return res.json({ success: false, msg: "Invalid url name format" });
      else queryObj.urlName = urlName.trim();

    //verifying blog fullName
    if (fullName) queryObj.fullName = fullName.trim();

    //checks if the blog in the params field actually exists
    Blog.findOne({ urlName: req.params.blogUrlName }, (err, blogToEdit) => {
      if (err) next(err);

      //blog doesn't exist
      if (!blogToEdit)
        return res.status(404).json({ success: false, msg: "Unknown blog." });

      //checks if user has rights to edit blog
      if (!blogToEdit.admins.includes(req.user._id))
        return res.status(401).json({ success: false, msg: "Unauthorized." });

      //checks if user requested to change UrlName
      if (queryObj.hasOwnProperty("urlName")) {
        //checks if the new urlName exists (it must be unique)
        Blog.findOne({ urlName: queryObj.urlName }, (err, blogFound) => {
          if (blogFound)
            res.status(401).json({
              success: false,
              msg: "New url name blog already exists.",
            });
          //urlName is unique. Blog can be edited
          else
            Blog.findByIdAndUpdate(
              blogToEdit._id,
              queryObj,
              (err, editedBlog) => {
                if (err) next(err);

                if (editedBlog) res.json({ success: true });
                else
                  return res.json({
                    success: false,
                    msg: "Could not edit blog",
                  });
              }
            );
        });
      } else
        Blog.findByIdAndUpdate(blogToEdit._id, queryObj, (err, editedBlog) => {
          if (err) next(err);

          if (editedBlog) res.json({ success: true });
          else res.json({ success: false, msg: "Could not edit blog" });
        });
    });
  }
);

//delete blog
router.delete(
  "/:blogUrlName",
  passport.authenticate("jwt", { session: false }),
  (req, res, next) => {
    Blog.findOne({ urlName: req.params.blogUrlName }, (err, blogToDelete) => {
      if (err) next(err);

      //blog doesn't exist
      if (!blogToDelete)
        return res.status(404).json({ success: false, msg: "Unknown blog." });
      else if (!blogToDelete.admins.includes(req.user._id))
        return res.status(401).json({ success: false, msg: "Unauthorized." });
      else
        Blog.findByIdAndDelete(blogToDelete._id, (err, blogDeleted) => {
          if (err) next(err);

          res.status(200).json({ success: true });
        });
    });
  }
);

//TODO: add /:blog/admin for add and remove admins from blog

module.exports = router;
