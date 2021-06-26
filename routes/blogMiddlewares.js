const blogUtils = require("../utils/blog");
const { checkField } = require("../utils/generic");

/**
 * Middeware for checking if a given blog exists in the database or not
 *
 * @param {String} field - name of the url name field
 * @param {String} where - Where to find field (params, query or body) in the req object
 */
module.exports.checkBlogExists = (field, where) => {
  return (req, res, next) => {
    blogUrlName = checkField(req, field, where);

    if (!blogUrlName)
      res.status(400).json({
        success: false,
        msg: "Missing required field",
      });
    else {
      blogUtils.blogExists(blogUrlName, (err, data) => {
        if (err) res.status(err.status).json(err.response);
        else {
          if (!req.checked) req.checked = {};

          req.checked.blog = data;
          next();
        }
      });
    }
  };
};

/**
 * Middeware for checking if a given tag exists in the database or not
 *
 * @param {(String|String[])} tagField - tag url name, or array of tag url names
 * @param {String} whereTagField - Where to find field (params, query or body) in the req object
 */
module.exports.checkTagExists = (tagField, whereTagField) => {
  return (req, res, next) => {
    tagUrlName = checkField(req, tagField, whereTagField);

    if (!tagUrlName)
      res.status(400).json({
        success: false,
        msg: "Missing required field",
      });
    else
      blogUtils.tagExists(
        Array.isArray(tagUrlName) ? tagUrlName : [tagUrlName],
        req.checked.blog,
        (err, data) => {
          if (err) res.status(err.status).json(err.response);
          else {
            req.checked.checked = data;
            next();
          }
        }
      );
  };
};
