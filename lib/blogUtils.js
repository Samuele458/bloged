const mongoose = require("mongoose");
const Blog = mongoose.model("Blog");
const Tag = mongoose.model("Tag");

/**
 * Callback for getting result
 *
 * @callback getResult
 * @param {object} err - If err, it is an object, otherwise it is null
 * @param {number} err.status - HTTP status for the response
 * @param {object} err.response - the response body
 * @param {boolean} err.response.success - True if success, false otherwise. However it will be always false, bacause it will be set only if error found
 * @param {string} err.response.msg - Response message that describes the error
 * @param {object} data - If no errors, data will contains the requested object
 */

/**
 * Check if blog exists
 *
 * @param {string} blogUrlName - The blog URL name
 * @param {getResult} callback - Callback to know if blog exists or not
 */
module.exports.blogExists = (blogUrlName, callback) => {
  Blog.findOne({ urlName: blogUrlName }, (err, data) => {
    if (typeof data === "undefined")
      callback(
        {
          status: 404,
          response: {
            success: false,
            msg: "Blog not found.",
          },
        },
        null
      );
    else callback(null, data);
  });
};

/**
 * Check if tag exists
 *
 * @param {string} blogUrlName - The blog URL name
 * @param {string} tagUrlName - The tag URL name
 * @param {getResult} callback - Callback to know if blog exists or not
 */
module.exports.tagExists = (tagUrlName, blogUrlName, callback) => {
  this.blogExists(blogUrlName, (err, data) => {
    if (err) callback(err, null);
    else {
      Tag.findOne(
        { urlName: tagUrlName, blog: mongoose.Types.ObjectId(data._id) },
        (err, data) => {
          if (typeof data === "undefined")
            callback(
              {
                status: 404,
                response: {
                  success: false,
                  msg: "Tag not found.",
                },
              },
              null
            );
        }
      );
    }
  });
};
