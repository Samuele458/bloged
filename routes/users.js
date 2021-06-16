const mongoose = require("mongoose");
const router = require("express").Router();
const User = mongoose.model("User");
const Verification = mongoose.model("Verification");
const passport = require("passport");
const authUtils = require("../lib/authUtils");
const crypto = require("crypto");

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
  let email = req.body.email.trim();
  let username = req.body.username.trim();
  let password = req.body.password;

  //checking for formats
  if (!authUtils.validateEmailFormat(email))
    return res.json({ success: false, msg: "invalid email format" });
  else if (!authUtils.validateUsernameFormat(username))
    return res.json({ success: false, msg: "invalid username format" });
  else if (!authUtils.validatePasswordFormat(password))
    return res.json({ success: false, msg: "invalid password format" });

  const saltHash = authUtils.hashPassword(req.body.password);

  const salt = saltHash.salt;
  const hash = saltHash.hash;

  User.find(
    { $or: [{ username: req.body.username }, { email: req.body.email }] },
    (err, data) => {
      if (err) next(err);

      if (data.length > 0) {
        if (data[0].username === req.body.username)
          return res.json({ success: false, msg: "username already exists" });
        if (data[0].email === req.body.email)
          return res.json({ success: false, msg: "email already exists" });
      }

      const newUser = new User({
        username: req.body.username,
        hash: hash,
        salt: salt,
        email: req.body.email,
      });

      newUser
        .save()
        .then((user) => {
          const newVerification = new Verification({
            verificationString: crypto.randomBytes(64).toString("hex"),
            user: user._id,
          });

          newVerification
            .save()
            .then((verification) => {
              authUtils.sendVerificationEmail(
                user.email,
                verification.verificationString,
                "localhost:3000"
              );
              res.json({
                success: true,
              });
            })
            .catch((err) => next(err));
        })
        .catch((err) => next(err));
    }
  );
});

router.get("/verify/:id", (req, res, next) => {
  console.log(req.params);
  Verification.findOneAndDelete({ verificationString: req.params.id })
    .populate("user")
    .exec((err, data) => {
      if (err) console.log(err);

      if (data) {
        User.findByIdAndUpdate(data.user.id, { active: true }, (err, user) => {
          if (err) console.log(err);

          res.json({ success: true, msg: "User's account verified." });
        });
      } else {
        res.json({ success: false, msg: "Expired verification" });
      }
    });
});

router.put("/:username");
module.exports = router;
