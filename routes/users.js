const mongoose = require("mongoose");
const router = require("express").Router();
const User = mongoose.model("User");
const Verification = mongoose.model("Verification");
const passport = require("passport");
const authUtils = require("../utils/auth");
const crypto = require("crypto");
const authMiddlewares = require("./authMiddlewares");
const { config } = require("../config");

router.post("/login", function (req, res, next) {
  let username = req.body.username;
  let email = req.body.email;
  let password = req.body.password;

  //object used to query user
  let queryObj = {};

  if (!password && !authUtils.validatePasswordFormat(password))
    return res.status(400).json({
      success: false,
      msg: "Invalid password format",
    });

  //only username or email
  if ((typeof email === "undefined") ^ (typeof username === "undefined")) {
    if (email) {
      if (authUtils.validateEmailFormat(email))
        queryObj = { email: email.trim() };
      else
        return res.status(400).json({
          success: false,
          msg: "Invalid email format",
        });
    } else if (authUtils.validateUsernameFormat(username)) {
      if (authUtils.validateUsernameFormat(username))
        queryObj = { username: username.trim() };
      else
        return res.status(400).json({
          success: false,
          msg: "Invalid username format",
        });
    }
  } else {
    //both email and username cannot be used toghether to login. Only one of them.
    return res.status(400).json({
      success: false,
      msg: "Use email or username to login.",
    });
  }

  User.findOne(queryObj)
    .then((user) => {
      if (!user) {
        res.status(404).json({ success: false, msg: "Unknown user" });
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
  let role = req.body.role || "subscriber";

  let roleLevel = authUtils.getRoleLevel(role);

  //check if roles exist
  if (typeof roleLevel === "undefined") {
    //unknown role
    return res.status(400).json({ success: false, msg: "Unknown role" });
  }

  if (req.authenticated) {
    let roleLevel = authUtils.getRoleLevel(req.user.role);
    //check if user has permissions to set the role
    if (reqRoleLevel <= roleLevel) {
      return res.status(401).json({ success: false, msg: "Unauthorized role" });
    }
  } else if (roleLevel > 1) {
    return res.status(401).json({ success: false, msg: "Unauthorized role" });
  }

  //checking for formats
  if (!authUtils.validateEmailFormat(email))
    return res
      .status(400)
      .json({ success: false, msg: "invalid email format" });
  else if (!authUtils.validateUsernameFormat(username))
    return res
      .status(400)
      .json({ success: false, msg: "invalid username format" });
  else if (!authUtils.validatePasswordFormat(password))
    return res
      .status(400)
      .json({ success: false, msg: "invalid password format" });

  const saltHash = authUtils.hashPassword(password);

  const salt = saltHash.salt;
  const hash = saltHash.hash;

  User.find(
    { $or: [{ username: username }, { email: email }] },
    (err, data) => {
      if (err) next(err);

      if (data.length > 0) {
        if (data[0].username === username)
          return res
            .status(400)
            .json({ success: false, msg: "username already exists" });
        if (data[0].email === email)
          return res
            .status(400)
            .json({ success: false, msg: "email already exists" });
      }

      const newUser = new User({
        username: username,
        hash: hash,
        salt: salt,
        email: email,
        role: role,
        active: process.env.NODE_ENV === "test", //if test, true
      });

      newUser
        .save()
        .then((user) => {
          //email verification is disabled in unit tests
          if (process.env.NODE_ENV === "test") {
            res.json({
              success: true,
            });
          } else {
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
          }
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

router.delete(
  "/:username",
  passport.authenticate("jwt", { session: false }),
  (req, res, next) => {
    if (req.params.username === req.user.username) {
      User.findByIdAndDelete(req.user._id, (err, userData) => {
        if (userData) res.status(200).json({ success: true });
        else res.status(404).json({ success: false, msg: "Unknown user" });
      });
    } else {
      res.status(401).json({ success: false, msg: "Unauthorized." });
    }
  }
);

module.exports = router;
