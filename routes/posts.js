const mongoose = require("mongoose");
const router = require("express").Router();
const passport = require("passport");
const authUtils = require("../lib/authUtils");
const blogMiddlewares = require("./blogMiddlewares");
const Tag = mongoose.model("Tag");
const Post = mongoose.model("Post");
const Blog = mongoose.model("Blog");

router.get(
  "/:blogUrlName/beh/:tagUrlName",
  blogMiddlewares.checkBlogExists("blogUrlName"),
  blogMiddlewares.checkTagExists("tagUrlName"),
  (req, res, next) => {
    res.json(req.checked);
  }
);

router.get("/:blogUrlName/posts/:postUrlName", (req, res, next) => {
  Blog.findOne({ urlName: req.params.blogUrlName }, (err, blog) => {
    if (err) next(err);

    //blog doesn't exist
    if (!blog)
      return res.status(404).json({ success: false, msg: "Unknown blog." });
    else
      Post.findOne(
        {
          urlName: req.params.postUrlName,
          blog: mongoose.Types.ObjectId(blog._id),
        },
        (err, post) => {
          if (err) next(err);

          if (post)
            res.status(200).json({
              success: true,
              data: {
                urlName: post.urlName,
                fullName: post.fullName,
                text: post.text,
              },
            });
          else res.status(404).json({ succes: false, msg: "Unknown post." });
        }
      );
  });
});

router.post(
  "/:blogUrlName/posts",
  passport.authenticate("jwt", { session: false }),
  (req, res, next) => {
    let urlName = req.body.urlName;
    let fullName = req.body.fullName;
    let text = req.body.text;
    let tags = req.body.tags;

    //checking formats
    if (!(urlName && authUtils.validateUrlnameFormat(urlName.trim())))
      return res
        .status(400)
        .json({ success: false, msg: "Invalid url name format" });
    else if (!fullName)
      return res
        .status(400)
        .json({ success: false, msg: "Invalid full name format" });

    //text is not necessary
    if (text) text = text.trim();

    urlName = urlName.trim();
    fullName = fullName.trim();
    if (!(Array.isArray(tags) || typeof tags === "undefined")) {
      res.status(400).json({ success: false, msg: "Invalid tags" });
    }

    Blog.findOne({ urlName: req.params.blogUrlName }, (err, blog) => {
      if (err) next(err);

      //check if blog exists
      if (!blog) res.status(404).json({ success: false, msg: "Unknown blog." });
      //checks if user has rights to edit blog
      else if (!blog.admins.includes(req.user._id))
        return res.status(401).json({ success: false, msg: "Unauthorized." });
      else
        Tag.find(
          {
            blog: mongoose.Types.ObjectId(blog._id),
            $or: tags.map((tag) => ({
              urlName: tag,
            })),
          },
          (err, tagsFound) => {
            if (err) next(err);

            //checking that all tags exists
            if (tagsFound.length !== tags.length)
              res.status(404).json({ success: false, msg: "Unknown tag" });
            else {
              Post.findOne(
                {
                  urlName: urlName,
                  blog: mongoose.Types.ObjectId(blog._id),
                },
                (err, postFound) => {
                  if (err) next(err);

                  if (postFound)
                    return res
                      .status(400)
                      .json({ success: false, msg: "Blog already exists" });

                  const newPost = new Post({
                    urlName: urlName,
                    fullName: fullName,
                    text: text,
                    blog: mongoose.Types.ObjectId(blog._id),
                    tags: tagsFound.map((tag) =>
                      mongoose.Types.ObjectId(tag._id)
                    ),
                  });

                  newPost.save((err, data) => {
                    if (err) next(err);

                    res.status(200).json({ success: true });
                  });
                }
              );
            }
          }
        );
    });
  }
);

router.delete(
  "/:blogUrlName/posts/:postUrlName",
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
        Post.findOneAndDelete(
          { urlName: req.params.postUrlName },
          (err, postDeleted) => {
            if (err) next(err);

            if (postDeleted) res.status(200).json({ success: true });
            else res.status(404).json({ succe: false, msg: "Unknown post." });
          }
        );
    });
  }
);

module.exports = router;
