const mongoose = require("mongoose");
const router = require("express").Router();
const User = mongoose.model("User");
const passport = require("passport");
const authUtils = require("../lib/authUtils");

router.post("/login", function (req, res, next) {
  User.findOne({ username: req.body.username })
    .then((user) => {
      if (!user) {
        res.status(401).json({ success: false, msg: "could not find user" });
      }

      const isValid = authUtils.validatePassword(
        req.body.password,
        user.hash,
        user.salt
      );

      if (isValid) {
        const jwt = authUtils.issueJWT(user);

        res.status(200).json({
          success: true,
          user: user,
          token: jwt.token,
          expiresIn: jwt.expires,
        });
      } else {
        res
          .status(401)
          .json({ success: false, msg: "you entered the wrong password" });
      }
    })
    .catch((err) => {
      next(err);
    });
});

// TODO
router.post("/register", function (req, res, next) {
  console.log(req.body.password);
  const saltHash = authUtils.hashPassword(req.body.password);

  const salt = saltHash.salt;
  const hash = saltHash.hash;

  User.find({ username: req.body.username }, (err, data) => {
    if (err) next(err);
    if (data.length) return res.json({ err: "username already exists" });

    const newUser = new User({
      username: req.body.username,
      hash: hash,
      salt: salt,
    });

    newUser
      .save()
      .then((user) => {
        const id = user._id;
        const jwt = authUtils.issueJWT(user);

        res.json({
          success: true,
          user: user,
          token: jwt.token,
          expiresIn: jwt.expires,
        });
      })
      .catch((err) => next(err));
  });
});

module.exports = router;
