const assert = require("chai").assert;
const blogUtils = require("../lib/blogUtils");
const mongoose = require("mongoose");
const Blog = mongoose.model("Blog");
const Tag = mongoose.model("Tag");

describe("Blog Utils tests", () => {
  before((done) => {
    newBlog = new Blog({
      urlName: "blog-utils",
      fullName: "Blog Utils",
    });

    newBlog.save((err, data) => {
      done();
    });
  });

  it("blogExists - Check for an existing blog", (done) => {
    blogUtils.blogExists("blog-utils", (err, data) => {
      assert.isNull(err);
      assert.equal(data.urlName, "blog-utils");
      console.log(data);
      done();
    });
  });

  it("blogExists - Check for a nonexistent blog", (done) => {
    done();
  });

  after((done) => {
    Blog.findOneAndDelete({ urlName: "blog-utils" }, (err, data) => {
      done();
    });
  });
});
