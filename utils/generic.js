const authUtils = require("./auth");

/**
 * Verification callback
 *
 * @callback verification
 * @param {*} value - Value passed to callback.
 * @returns a boolean, in order to decide if check passed or not
 */

/**
 * Check if a given field is present or not in the request object
 *
 * @param {object} req - Request object
 * @param {string} field - Field name to check
 * @param {string} where - Location in which find the field (body, params, etc.)
 * @param {(verification|string|null)} verification - Field to check if the value is correct or not. If null, it just returns the value. If string it executes a pre-existing check (like email verification). Also, it can be a function, in order to create a custom check. If it returns true the value is correct, otherwise null is returned
 * @returns the field data if it is present, otherwise it returns undefined
 */
const checkField = (req, field, where, verification) => {
  let locations = ["query", "params", "body"];

  let fieldValue;

  //checking if location exists
  if (locations.indexOf(where) !== -1) {
    if (typeof where !== "undefined") {
      fieldValue = req[where][field];
    }
  }

  //if no verification provided, return fieldValue (null if it is undefined)
  if (typeof verification === "undefined")
    if (typeof fieldValue !== "undefined") return fieldValue;
    else return null;

  //preset verification
  if (typeof verification === "string") {
    if (typeof fieldValue === "undefined") {
      //if value is undefined return null
      return null;
    } else if (verification === "email") {
      //check if email
      if (authUtils.validateEmailFormat(fieldValue)) return fieldValue.trim();
    } else if (verification == "urlName") {
      //check if urlName
      if (authUtils.validateUrlnameFormat(fieldValue)) return fieldValue.trim();
    } else if (verification == "fullName") {
      //check if fullName
      return fieldValue.trim();
    } else if (verification == "text") {
      //checking for common text
      return fieldValue.trim();
    }

    return null;
  }

  //custom verification
  if (typeof verification === "function") {
    //executing verification
    if (verification(fieldValue)) return fieldValue;
    else return null; //verification failed
  }
};

module.exports.checkField = checkField;
