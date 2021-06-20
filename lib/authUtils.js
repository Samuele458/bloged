const crypto = require("crypto");
const jsonwebtoken = require("jsonwebtoken");
const fs = require("fs");
const path = require("path");
const emailUtils = require("./emailUtils");
const { config } = require("../config");

const pathToPrivateKey = path.join(__dirname, "..", "id_rsa_priv.pem");
const PRIV_KEY = fs.readFileSync(pathToPrivateKey, "utf8");

//check if hash correspond to password
function validatePassword(password, hash, salt) {
  var hashVerify = crypto
    .pbkdf2Sync(password, salt, 10000, 64, "sha512")
    .toString("hex");
  return hash === hashVerify;
}

//generate hash and salt
function hashPassword(password) {
  var salt = crypto.randomBytes(32).toString("hex");
  var genHash = crypto
    .pbkdf2Sync(password, salt, 10000, 64, "sha512")
    .toString("hex");

  return {
    salt: salt,
    hash: genHash,
  };
}

function issueJWT(user) {
  const _id = user._id;

  const expiresIn = "1d";

  const payload = {
    sub: _id,
    iat: Date.now(),
  };

  const signedToken = jsonwebtoken.sign(payload, PRIV_KEY, {
    expiresIn: expiresIn,
    algorithm: "RS256",
  });

  return {
    token: "Bearer " + signedToken,
    expires: expiresIn,
  };
}

//check if email is valid or not
function validateEmailFormat(email) {
  const emailRegex =
    /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

  return emailRegex.test(email);
}

//check if passsword is valid or not
function validatePasswordFormat(password) {
  const pwRegex = /^.{8,}$/;

  return pwRegex.test(password);
}

//check if username is valid or not
function validateUsernameFormat(username) {
  const usernameRegex = /^[a-zA-Z0-9-_\.]+$/;

  return usernameRegex.test(username);
}

//check if urlName is valid or not
function validateUrlnameFormat(urlName) {
  const urlnameRegex = /^[a-zA-Z0-9\-_]{3,}$/;

  return urlnameRegex.test(urlName);
}

function sendVerificationEmail(to, verificationString, hostname) {
  emailUtils.sendEmail(
    to,
    "Verify your account",
    `Verify your account: <a href="http://${hostname}/users/verify/${verificationString}">here</a>`
  );
}

//returns level of role. "undefined" if not found
function getRoleLevel(role) {
  let returnValue = undefined;
  config.roles.forEach((e) => {
    if (e.role == role) returnValue = e.level;
  });
  return returnValue;
}

module.exports.hashPassword = hashPassword;
module.exports.issueJWT = issueJWT;
module.exports.validatePassword = validatePassword;
module.exports.validateEmailFormat = validateEmailFormat;
module.exports.validatePasswordFormat = validatePasswordFormat;
module.exports.validateUsernameFormat = validateUsernameFormat;
module.exports.sendVerificationEmail = sendVerificationEmail;
module.exports.getRoleLevel = getRoleLevel;
module.exports.validateUrlnameFormat = validateUrlnameFormat;
