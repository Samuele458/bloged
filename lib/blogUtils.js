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
    console.log(typeof data);
    if (data) callback(null, data);
    else
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
  });
};

/**
 * Check if tag exists
 *
 * @param {(string|object)} blogUrlName - The blog URL name
 * @param {string[]} tagsUrlName - Array of tag URL names
 * @param {getResult} callback - Callback to know if blog exists or not
 */

module.exports.tagExists = (tagsUrlName, blog, callback) => {
  //function for finding tag
  const findTag = (blogData) => {
    Tag.find(
      {
        $or: tagsUrlName.map((tag) => ({
          urlName: tag,
        })),
        blog: mongoose.Types.ObjectId(blogData._id),
      },
      (err, tagsData) => {
        if (tagsData.length !== tagsUrlName.length)
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
        else callback(null, tagsData);
      }
    );
  };

  if (typeof blog === "string")
    //fetch blog data before
    this.blogExists(blogUrlName, (err, data) => {
      if (err) callback(err, null);
      else {
        findTag(data);
      }
    });
  else {
    //passing blog data
    findTag(blog);
  }
};
