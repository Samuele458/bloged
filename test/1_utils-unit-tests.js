const assert = require("chai").assert;
const app = require("../index.js");
const authUtils = require("../lib/authUtils");

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
