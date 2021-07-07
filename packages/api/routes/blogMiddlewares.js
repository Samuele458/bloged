const blogUtils = require("../utils/blog");
const { checkField } = require("../utils/generic");
const { BadRequestError, UnauthorizedError } = require("../utils/errors");

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
      return next(new BadRequestError("Missing required field"));
    else {
      blogUtils.blogExists(blogUrlName, (err, data) => {
        if (err) return next(err);
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

    if (!tagUrlName) return next(new BadRequestError("Missing required field"));
    else
      blogUtils.tagExists(
        Array.isArray(tagUrlName) ? tagUrlName : [tagUrlName],
        req.checked.blog,
        (err, data) => {
          if (err) return next(err);
          else {
            req.checked.tags = data;
            next();
          }
        }
      );
  };
};

/**
 * Middeware for checking if a given category exists in the database or not
 *
 * @param {(String|String[])} categoryField - category url name, or array of category url names
 * @param {String} whereCategoryField - Where to find field (params, query or body) in the req object
 */
module.exports.checkCategoryExists = (categoryField, whereCategoryField) => {
  return (req, res, next) => {
    categoryUrlName = checkField(req, categoryField, whereCategoryField);

    if (!categoryUrlName)
      return next(new BadRequestError("Missing required field"));
    else
      blogUtils.categoryExists(
        Array.isArray(categoryUrlName) ? categoryUrlName : [categoryUrlName],
        req.checked.blog,
        (err, data) => {
          if (err) return next(err);
          else {
            req.checked.categories = data;
            next();
          }
        }
      );
  };
};

/**
 * Middeware for checking if a given post exists in the database or not
 *
 * @param {(String|String[])} postField - post url name, or array of post url names
 * @param {String} wherePostField - Where to find field (params, query or body) in the req object
 */
module.exports.checkPostExists = (postField, wherePostField) => {
  return (req, res, next) => {
    postUrlName = checkField(req, postField, wherePostField);

    if (!postUrlName)
      return next(new BadRequestError("Missing required field"));
    else
      blogUtils.postExists(
        Array.isArray(postUrlName) ? postUrlName : [postUrlName],
        req.checked.blog,
        (err, data) => {
          if (err) return next(err);
          else {
            req.checked.posts = data;
            next();
          }
        }
      );
  };
};

module.exports.checkImageExists = (field, where) => {
  return (req, res, next) => {
    imageUrlName = checkField(req, field, where);

    if (!imageUrlName)
      return next(new BadRequestError("Missing required field"));
    else
      blogUtils.imageExists(
        Array.isArray(imageUrlName) ? imageUrlName : [imageUrlName],
        req.checked.blog,
        (err, data) => {
          if (err) return next(err);
          else {
            req.checked.images = data;
            next();
          }
        }
      );
  };
};

module.exports.userIsAdmin = (req, res, next) => {
  if (req.checked.blog.admins.indexOf(req.user._id) !== -1) next();
  else next(new UnauthorizedError());
};