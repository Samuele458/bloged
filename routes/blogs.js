const mongoose = require("mongoose");
const router = require("express").Router();
const passport = require("passport");
const authUtils = require("../lib/authUtils");
const Blog = mongoose.model("Blog");

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
        $push: { admins: mongoose.Types.ObjectId(admin) },
      });

      newBlog.save((err, data) => {
        if (err) next(err);
        console.log(data);
        res.send(data);
      });
    });
  }
);

module.exports = router;
