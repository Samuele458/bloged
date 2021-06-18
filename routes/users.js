const mongoose = require("mongoose");
const router = require("express").Router();
const User = mongoose.model("User");
const Verification = mongoose.model("Verification");
const passport = require("passport");
const authUtils = require("../lib/authUtils");
const crypto = require("crypto");
const authMiddlewares = require("./authMiddlewares");
const { config } = require("../config");

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
  let role = req.body.role || "subscriber";

  let roleLevel = authUtils.getRoleLevel(role);

  //check if roles exist
  if (typeof roleLevel === "undefined") {
    //unknown role
    return res.status(401).json({ success: false, msg: "Unknown role" });
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
    return res.json({ success: false, msg: "invalid email format" });
  else if (!authUtils.validateUsernameFormat(username))
    return res.json({ success: false, msg: "invalid username format" });
  else if (!authUtils.validatePasswordFormat(password))
    return res.json({ success: false, msg: "invalid password format" });

  const saltHash = authUtils.hashPassword(password);

  const salt = saltHash.salt;
  const hash = saltHash.hash;

  User.find(
    { $or: [{ username: username }, { email: email }] },
    (err, data) => {
      if (err) next(err);

      if (data.length > 0) {
        if (data[0].username === username)
          return res.json({ success: false, msg: "username already exists" });
        if (data[0].email === email)
          return res.json({ success: false, msg: "email already exists" });
      }

      const newUser = new User({
        username: username,
        hash: hash,
        salt: salt,
        email: email,
        role: role,
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

//edit
router.get("/hello", authMiddlewares.optionalAuthenticate, (req, res) => {
  //console.log(req);
  res.send(req.authenticated);
});
module.exports = router;
