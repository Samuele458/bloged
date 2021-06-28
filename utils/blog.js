const mongoose = require("mongoose");
const Blog = mongoose.model("Blog");
const Tag = mongoose.model("Tag");
const Post = mongoose.model("Post");
const Image = mongoose.model("Image");
const { NotFoundError } = require("../utils/errors");

/**
 * Callback for getting result
 *
 * @callback getResult
 * @param {(Error|null)} err - If err, it is an Error object, otherwise it is null
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
    if (data) callback(null, data);
    else callback(new NotFoundError("Blog not found"), null);
  });
};

/**
 * Check if tags exist
 *
 * @param {(string|object)} blogUrlName - The blog URL name, or the blog data
 * @param {string[]} tagsUrlName - Array of tag URL names to check
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
          callback(new NotFoundError("Tag not found."), null);
        else callback(null, tagsData);
      }
    );
  };

  if (typeof blog === "string")
    //fetch blog data before
    this.blogExists(blog, (err, data) => {
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

/**
 * Check if post exists
 *
 * @param {(string|object)} blogUrlName - The blog URL name, or the blog data
 * @param {string[]} postsUrlName - Array of tag URL names to check
 * @param {getResult} callback - Callback to know if blog exists or not
 */
module.exports.postExists = (postsUrlName, blog, callback) => {
  //function for finding post
  const findPost = (blogData) => {
    Post.find(
      {
        $or: postsUrlName.map((tag) => ({
          urlName: tag,
        })),
        blog: mongoose.Types.ObjectId(blogData._id),
      },
      (err, postsData) => {
        if (postsData.length !== postsUrlName.length)
          callback(new NotFoundError("Post not found."), null);
        else callback(null, postsData);
      }
    );
  };

  if (typeof blog === "string")
    //fetch blog data before
    this.blogExists(blog, (err, data) => {
      if (err) callback(err, null);
      else {
        findPost(data);
      }
    });
  else {
    //passing blog data
    findPost(blog);
  }
};

/**
 * Check if images exist
 *
 * @param {(string|object)} blogUrlName - The blog URL name, or the blog data
 * @param {string[]} imageUrlName - Array of image URL names to check
 * @param {getResult} callback - Callback to know if blog exists or not
 */
module.exports.imageExists = (imageUrlName, blog, callback) => {
  //function for finding image
  const findImage = (blogData) => {
    Image.find(
      {
        $or: imageUrlName.map((image) => ({
          urlName: image,
        })),
        blog: mongoose.Types.ObjectId(blogData._id),
      },
      (err, imagesData) => {
        if (err) return next(err);

        if (imagesData.length !== imageUrlName.length)
          callback(new NotFoundError("Image not found."), null);
        else callback(null, imagesData);
      }
    );
  };

  if (typeof blog === "string")
    //fetch blog data before
    this.blogExists(blog, (err, data) => {
      if (err) callback(err, null);
      else {
        findImage(data);
      }
    });
  else {
    //passing blog data
    findImage(blog);
  }
};
