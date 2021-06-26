const assert = require("chai").assert;
const blogUtils = require("../utils/blog");
const mongoose = require("mongoose");
const Blog = mongoose.model("Blog");
const Tag = mongoose.model("Tag");

describe("Blog Utils tests", () => {
  before((done) => {
    let newBlog = new Blog({
      urlName: "blog-utils",
      fullName: "Blog Utils",
    });

    newBlog.save((err, blogData) => {
      let newTag = new Tag({
        urlName: "tag1",
        fullName: "Tag number 1",
        blog: blogData._id,
      });

      newTag.save((err, tagData) => {
        done();
      });
    });
  });

  it("blogExists - Check for an existing blog", (done) => {
    blogUtils.blogExists("blog-utils", (err, data) => {
      assert.isNull(err);
      assert.equal(data.urlName, "blog-utils");
      done();
    });
  });

  it("blogExists - Check for a nonexistent blog", (done) => {
    blogUtils.blogExists("blog-unknown", (err, data) => {
      assert.isNull(data);
      assert.equal(err.status, 404);
      done();
    });
  });

  it("tagExists - Check for an existing tag", (done) => {
    blogUtils.tagExists(["tag1"], "blog-utils", (err, data) => {
      assert.isNull(err);
      assert.equal(data[0].urlName, "tag1");
      done();
    });
  });

  it("tagExists - Check for a nonexistent tag", (done) => {
    blogUtils.tagExists(["tag13"], "blog-utils", (err, data) => {
      assert.isNull(data);
      assert.equal(err.status, 404);
      done();
    });
  });

  after((done) => {
    Blog.findOneAndDelete({ urlName: "blog-utils" }, (err, data) => {
      Tag.findOneAndDelete({ urlName: "tag1" }, (err, data) => {
        done();
      });
    });
  });
});
