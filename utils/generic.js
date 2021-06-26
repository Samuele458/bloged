/**
 * Check if a given field is present or not in the request object
 *
 * @param {*} req - Request object
 * @param {*} field - Field name to check
 * @param {*} where - Location in which find the field (body, params, etc.)
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

  //if verification is a string
  if (typeof verification === "string") {
    if (verification === "email") {
      emailRegex =
        /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

      if (emailRegex.test(email)) return fieldValue;
      else return null;
    } else return null;
  }

  if (typeof verification === "function") {
    //executing verification
    if (verification(fieldValue)) return fieldValue;
    else return null; //verification failed
  }
};

module.exports.checkField = checkField;
