const assert = require("chai").assert;
const app = require("../index.js");
const authUtils = require("../utils/auth");
const blogUtils = require("../utils/blog");
const mongoose = require("mongoose");
const Blog = mongoose.model("Blog");
const Tag = mongoose.model("Tag");

describe("Utils", () => {
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
        assert.equal(err.statusCode, 404);
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
        assert.equal(err.statusCode, 404);
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

  describe("Credentials format", () => {
    it("Validation email format", () => {
      const validEmails = [
        "email@example.com",
        "firstname.lastname@example.com",
        "email@subdomain.example.com",
        "email@[123.123.123.123]",
        '"email"@example.com',
        "1234567890@example.com",
        "email@example-one.com",
        "email@example.name",
        "email@example.museum",
        "email@example.co.jp",
        "firstname-lastname@example.com",
      ];

      validEmails.forEach((email) => {
        assert.isTrue(authUtils.validateEmailFormat(email));
      });

      const invalidEmails = [
        "plainaddress",
        "#@%^%#$@#$@#.com",
        "@example.com",
        "Joe Smith <email@example.com>",
        "email.example.com",
        "email@example@example.com",
        ".email@example.com",
        "email.@example.com",
        "email..email@example.com",
        "email@example.com (Joe Smith)",
        "email@example",
        "email@111.222.333.44444",
        "email@example..com",
        "Abc..123@example.com",
      ];

      invalidEmails.forEach((email) => {
        assert.isFalse(authUtils.validateEmailFormat(email));
      });
    });

    it("Validation password format", () => {
      assert.isTrue(authUtils.validatePasswordFormat("12345678"));
      assert.isTrue(authUtils.validatePasswordFormat("rwerw267"));
      assert.isTrue(authUtils.validatePasswordFormat("rwerw267@23gd///2332"));
      assert.isFalse(authUtils.validatePasswordFormat(""));
      assert.isFalse(authUtils.validatePasswordFormat("123"));
      assert.isFalse(authUtils.validatePasswordFormat("123sdsd"));
    });

    it("Validation username format", () => {
      assert.isTrue(authUtils.validateUsernameFormat("12345678"));
      assert.isTrue(authUtils.validateUsernameFormat("rwerw267"));
      assert.isFalse(authUtils.validateUsernameFormat("rwerw267@23gd///2332"));
      assert.isFalse(authUtils.validateUsernameFormat(""));
      assert.isTrue(authUtils.validateUsernameFormat("123"));
      assert.isTrue(authUtils.validateUsernameFormat("123sdsd"));
    });
  });

  describe("Permissions tests", () => {
    it("GetRoleLevel() returns correct permission levels", () => {
      assert.isNotFalse(authUtils.getRoleLevel("admin")); //admin probably will be removed
      assert.isNotFalse(authUtils.getRoleLevel("subscriber"));
      assert.isNotFalse(authUtils.getRoleLevel("owner"));

      assert.isTrue(
        authUtils.getRoleLevel("owner") > authUtils.getRoleLevel("admin")
      );

      assert.isTrue(
        authUtils.getRoleLevel("admin") > authUtils.getRoleLevel("subscriber")
      );
    });
  });
});
