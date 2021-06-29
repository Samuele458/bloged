const chaiHttp = require("chai-http");
const chai = require("chai");
const assert = chai.assert;
const server = require("../index");
const { request } = require("chai");

chai.use(chaiHttp);

//waiting for mongoDB connection
before(function (done) {
  this.timeout(50000);
  console.log("Waiting for mongoDB connection...");
  server.on("ready", () => {
    done();
  });
});

describe("REST API tests", () => {
  describe("Users authentication", () => {
    //variable used for JWT authorization
    let token;

    it("POST /users/register - Valid credentials", (done) => {
      chai
        .request(server)
        .post("/users/register")
        .send({
          email: "exam.ple281636@example.com",
          username: "SampleUser23",
          password: "c0mmon_pw123@",
        })
        .end((err, res) => {
          assert.equal(res.status, 200);
          assert.equal(res.body.success, true);
          done();
        });
    });

    it("POST /users/register - Wrong email format", (done) => {
      chai
        .request(server)
        .post("/users/register")
        .send({
          email: "exam.ple281636example.com",
          username: "SampleUser23",
          password: "c0mmon_pw123@",
        })
        .end((err, res) => {
          assert.equal(res.status, 400);
          assert.equal(res.body.success, false);
          done();
        });
    });

    it("POST /users/register - Wrong password format", (done) => {
      chai
        .request(server)
        .post("/users/register")
        .send({
          email: "exam.ple281636@example.com",
          username: "SampleUser23",
          password: "c",
        })
        .end((err, res) => {
          assert.equal(res.status, 400);
          assert.equal(res.body.success, false);
          done();
        });
    });

    it("POST /users/register - Wrong username format", (done) => {
      chai
        .request(server)
        .post("/users/register")
        .send({
          email: "exam.ple281636@example.com",
          username: "John^^",
          password: "c2434jhd",
        })
        .end((err, res) => {
          assert.equal(res.status, 400);
          assert.equal(res.body.success, false);
          done();
        });
    });

    it("POST /users/register - Username already exists", (done) => {
      chai
        .request(server)
        .post("/users/register")
        .send({
          email: "exam.ple281636@example.com",
          username: "SampleUser23",
          password: "c2434jhd",
        })
        .end((err, res) => {
          assert.equal(res.status, 400);
          assert.equal(res.body.success, false);
          done();
        });
    });

    it("POST /users/login - Wrong username", (done) => {
      chai
        .request(server)
        .post("/users/login")
        .send({
          username: "randomUser",
          password: "c0mmon_pw123@",
        })
        .end((err, res) => {
          assert.equal(res.status, 404);
          assert.equal(res.body.success, false);
          token = res.body.token;
          done();
        });
    });

    it("POST /users/login - Wrong email", (done) => {
      chai
        .request(server)
        .post("/users/login")
        .send({
          email: "exam.ple2d81636@example.com",
          password: "c0mmon_pw123@",
        })
        .end((err, res) => {
          assert.equal(res.status, 404);
          assert.equal(res.body.success, false);
          token = res.body.token;
          done();
        });
    });

    it("POST /users/login - Login by username", (done) => {
      chai
        .request(server)
        .post("/users/login")
        .send({
          username: "SampleUser23",
          password: "c0mmon_pw123@",
        })
        .end((err, res) => {
          assert.equal(res.status, 200);
          assert.equal(res.body.success, true);
          token = res.body.token;

          done();
        });
    });

    it("POST /users/login - Login by email", (done) => {
      chai
        .request(server)
        .post("/users/login")
        .send({
          email: "exam.ple281636@example.com",
          password: "c0mmon_pw123@",
        })
        .end((err, res) => {
          assert.equal(res.status, 200);
          assert.equal(res.body.success, true);
          token = res.body.token;
          done();
        });
    });

    it("POST /users/login - Provide both email and username", (done) => {
      chai
        .request(server)
        .post("/users/login")
        .send({
          email: "exam.ple281636@example.com",
          username: "SampleUser23",
          password: "c0mmon_pw123@",
        })
        .end((err, res) => {
          assert.equal(res.status, 400);
          assert.equal(res.body.success, false);
          done();
        });
    });

    it("DELETE /users/:username - Trying to delete an unknown user without authorization", (done) => {
      chai
        .request(server)
        .delete("/users/randomUser382")
        .end((err, res) => {
          assert.equal(res.status, 401);
          done();
        });
    });

    it("DELETE /users/:username - Trying to delete an unknown user with authorization", (done) => {
      chai
        .request(server)
        .delete("/users/randomUser382")
        .set({ Authorization: token })
        .end((err, res) => {
          assert.equal(res.status, 404);
          assert.equal(res.body.success, false);
          done();
        });
    });

    it("DELETE /users/:username - Delete the owned user", (done) => {
      chai
        .request(server)
        .delete("/users/SampleUser23")
        .set({ Authorization: token })
        .end((err, res) => {
          assert.equal(res.status, 200);
          assert.equal(res.body.success, true);
          done();
        });
    });
  });

  describe("Blog", () => {
    let token;
    before((done) => {
      chai
        .request(server)
        .post("/users/register")
        .send({
          email: "bloguser8726472@blog.example.com",
          username: "blogUser",
          password: "random12345",
        })
        .end((err, res) => {
          if (res.status === 200)
            chai
              .request(server)
              .post("/users/login")
              .send({
                username: "blogUser",
                password: "random12345",
              })
              .end((err, res) => {
                if (res.status === 200) {
                  token = res.body.token;
                  done();
                }
              });
        });
    });

    it("POST /blogs/ - Trying to create blog without authorization", (done) => {
      chai
        .request(server)
        .post("/blogs")
        .send({
          urlName: "tech-blog",
          fullName: "TheTechBlog",
        })
        .end((err, res) => {
          assert.equal(res.status, 401);
          done();
        });
    });

    it("POST /blogs/ - Invalid url name blog", (done) => {
      chai
        .request(server)
        .post("/blogs")
        .set({ Authorization: token })
        .send({
          urlName: "tech@blog",
          fullName: "TheTechBlog",
        })
        .end((err, res) => {
          assert.equal(res.status, 400);
          assert.isFalse(res.body.success);
          done();
        });
    });

    it("POST /blogs/ - Create new blog", (done) => {
      chai
        .request(server)
        .post("/blogs")
        .set({ Authorization: token })
        .send({
          urlName: "tech-blog",
          fullName: "TheTechBlog",
        })
        .end((err, res) => {
          assert.equal(res.status, 200);
          assert.isTrue(res.body.success);
          done();
        });
    });

    it("POST /blogs/ - Trying to create blog with existing name", (done) => {
      chai
        .request(server)
        .post("/blogs")
        .set({ Authorization: token })
        .send({
          urlName: "tech-blog",
          fullName: "TheTechBlog",
        })
        .end((err, res) => {
          assert.equal(res.status, 400);
          assert.isFalse(res.body.success);
          done();
        });
    });

    it("PUT /blogs/:blogUrlName - Trying to edit blog without authorization", (done) => {
      chai
        .request(server)
        .put("/blogs/tech-blog")
        .send({
          urlName: "tech-blog-edit",
          fullName: "TheTechBlog2",
        })
        .end((err, res) => {
          assert.equal(res.status, 401);
          done();
        });
    });

    it("PUT /blogs/:blogUrlName - Edit blog", (done) => {
      chai
        .request(server)
        .put("/blogs/tech-blog")
        .set({ Authorization: token })
        .send({
          urlName: "tech-blog-edit",
          fullName: "TheTechBlog2",
        })
        .end((err, res) => {
          assert.equal(res.status, 200);
          assert.isTrue(res.body.success);
          done();
        });
    });

    it("GET /blogs/:blogUrlName - Get blog information", (done) => {
      chai
        .request(server)
        .get("/blogs/tech-blog-edit")
        .end((err, res) => {
          assert.equal(res.status, 200);
          assert.isTrue(res.body.success);
          assert.equal(res.body.data.urlName, "tech-blog-edit");
          assert.equal(res.body.data.fullName, "TheTechBlog2");
          done();
        });
    });

    it("GET /blogs/:blogUrlName - Trying to get an unknown blog", (done) => {
      chai
        .request(server)
        .get("/blogs/tech-blog")
        .end((err, res) => {
          assert.equal(res.status, 404);
          assert.isFalse(res.body.success);
          done();
        });
    });

    it("DELETE /blogs/:blogUrlName - Trying to delete an unknown blog", (done) => {
      chai
        .request(server)
        .delete("/blogs/tech-blog")
        .set({ Authorization: token })
        .end((err, res) => {
          assert.equal(res.status, 404);
          assert.isFalse(res.body.success);
          done();
        });
    });

    it("DELETE /blogs/:blogUrlName - Trying to delete a blog without authorization", (done) => {
      chai
        .request(server)
        .delete("/blogs/tech-blog-edit")
        .end((err, res) => {
          assert.equal(res.status, 401);
          done();
        });
    });

    it("DELETE /blogs/:blogUrlName - Delete a blog", (done) => {
      chai
        .request(server)
        .delete("/blogs/tech-blog-edit")
        .set({ Authorization: token })
        .end((err, res) => {
          assert.equal(res.status, 200);
          assert.isTrue(res.body.success);
          done();
        });
    });

    after((done) => {
      chai
        .request(server)
        .delete("/users/blogUser")
        .set({ Authorization: token })
        .end((err, res) => {
          assert.equal(res.status, 200);
          assert.equal(res.body.success, true);
          done();
        });
    });
  });

  describe("Tags", () => {
    let token;
    before((done) => {
      chai
        .request(server)
        .post("/users/register")
        .send({
          email: "test.user837263@tst.example.co",
          username: "testUser",
          password: "1234@qwertdfg",
        })
        .end((err, res) => {
          chai
            .request(server)
            .post("/users/login")
            .send({
              username: "testUser",
              password: "1234@qwertdfg",
            })
            .end((err, res) => {
              token = res.body.token;

              chai
                .request(server)
                .post("/blogs")
                .set({ Authorization: token })
                .send({
                  urlName: "test-blog",
                  fullName: "The Test Blog",
                })
                .end((err, res) => {
                  done();
                });
            });
        });
    });

    it("POST /:blogUrlName/tags/ - Create new tag without authorization", (done) => {
      chai
        .request(server)
        .post("/blogs/test-blog/tags")
        .send({
          urlName: "new",
          fullName: "New product",
        })
        .end((err, res) => {
          assert.equal(res.status, 401);
          done();
        });
    });

    it("POST /blogs/:blogUrlName/tags/ - Create new tag", (done) => {
      chai
        .request(server)
        .post("/blogs/test-blog/tags")
        .set({ Authorization: token })
        .send({
          urlName: "new",
          fullName: "New product",
        })
        .end((err, res) => {
          assert.equal(res.status, 200);
          assert.isTrue(res.body.success);
          done();
        });
    });

    it("POST /blogs/:blogUrlName/tags/ - Create new tag without fields", (done) => {
      chai
        .request(server)
        .post("/blogs/test-blog/tags")
        .set({ Authorization: token })
        .send({})
        .end((err, res) => {
          assert.equal(res.status, 400);
          assert.isFalse(res.body.success);
          done();
        });
    });

    it("POST /blogs/:blogUrlName/tags/ - Create new tag with description", (done) => {
      chai
        .request(server)
        .post("/blogs/test-blog/tags")
        .set({ Authorization: token })
        .send({
          urlName: "new-topics",
          fullName: "New topics",
          description: "Check out new topics!",
        })
        .end((err, res) => {
          assert.equal(res.status, 200);
          assert.isTrue(res.body.success);
          done();
        });
    });

    it("PUT /blogs/:blogUrlName/tags/ - Trying to edit tag without authorization", (done) => {
      chai
        .request(server)
        .put("/blogs/test-blog/tags/new-topics")
        .send({
          urlName: "another-tag",
          fullName: "Other stuff",
          description: "Check out other tag",
        })
        .end((err, res) => {
          assert.equal(res.status, 401);
          done();
        });
    });

    it("PUT /blogs/:blogUrlName/tags/ - Trying to edit an unknown tag", (done) => {
      chai
        .request(server)
        .put("/blogs/test-blog/tags/unknown")
        .set({ Authorization: token })
        .send({
          urlName: "another-tag",
          fullName: "Other stuff",
          description: "Check out other tag",
        })
        .end((err, res) => {
          assert.equal(res.status, 404);
          done();
        });
    });

    it("PUT /blogs/:blogUrlName/tags/ - Edit tag", (done) => {
      chai
        .request(server)
        .put("/blogs/test-blog/tags/new-topics")
        .set({ Authorization: token })
        .send({
          urlName: "another-tag",
          fullName: "Other stuff",
          description: "Check out other tag",
        })
        .end((err, res) => {
          assert.equal(res.status, 200);
          assert.isTrue(res.body.success);
          done();
        });
    });

    it("GET /blog/:blogUrlName/tags/:tagUrlName - Get tag info", (done) => {
      chai
        .request(server)
        .get("/blogs/test-blog/tags/new")
        .end((err, res) => {
          assert.equal(res.status, 200);
          assert.equal(res.body.data.urlName, "new");
          assert.equal(res.body.data.fullName, "New product");
          done();
        });
    });

    it("DELETE /blogs/:blogUrlName/tags/:tagUrlName - Delete tag", (done) => {
      chai
        .request(server)
        .delete("/blogs/test-blog/tags/new")
        .set({ Authorization: token })
        .end((err, res) => {
          assert.equal(res.status, 200);
          assert.isTrue(res.body.success);

          chai
            .request(server)
            .delete("/blogs/test-blog/tags/another-tag")
            .set({ Authorization: token })
            .end((err, res) => {
              assert.equal(res.status, 200);
              assert.isTrue(res.body.success);
              done();
            });
        });
    });

    after((done) => {
      chai
        .request(server)
        .delete("/blogs/test-blog")
        .set({ Authorization: token })
        .end((err, res) => {
          chai
            .request(server)
            .delete("/users/testUser")
            .set({ Authorization: token })
            .end((err, res) => {
              if (res.status === 200) done();
            });
        });
    });
  });

  describe("Categories", () => {
    let token;
    before((done) => {
      chai
        .request(server)
        .post("/users/register")
        .send({
          email: "test.user8372634@tst.example.co",
          username: "testUser2",
          password: "1234@qwertdfg",
        })
        .end((err, res) => {
          chai
            .request(server)
            .post("/users/login")
            .send({
              username: "testUser2",
              password: "1234@qwertdfg",
            })
            .end((err, res) => {
              token = res.body.token;

              chai
                .request(server)
                .post("/blogs")
                .set({ Authorization: token })
                .send({
                  urlName: "blogtest",
                  fullName: "The Test Blog",
                })
                .end((err, res) => {
                  done();
                });
            });
        });
    });

    it("POST /blogs/:blogUrlName/categories/ - Trying to create category without authorization", (done) => {
      chai
        .request(server)
        .post("/blogs/blogtest/categories")
        .send({
          urlName: "tech",
          fullName: "Tech products",
        })
        .end((err, res) => {
          assert.equal(res.status, 401);
          done();
        });
    });

    it("POST /blogs/:blogUrlName/categories/ - Create new category", (done) => {
      chai
        .request(server)
        .post("/blogs/blogtest/categories")
        .set({ Authorization: token })
        .send({
          urlName: "tech",
          fullName: "Tech products",
        })
        .end((err, res) => {
          assert.equal(res.status, 200);
          assert.isTrue(res.body.success);
          done();
        });
    });


    it("PUT /blogs/:blogUrlName/categories/:categoryUrlName - Trying to edit category without authorization", (done) => {
      chai
        .request(server)
        .put("/blogs/blogtest/categories/tech")
        .send({
          urlName: "tech3",
        })
        .end((err, res) => {
          assert.equal(res.status, 401);
          done();
        });
    });

    it("PUT /blogs/:blogUrlName/categories/:categoryUrlName - Edit category", (done) => {
      chai
        .request(server)
        .put("/blogs/blogtest/categories/tech")
        .set({ Authorization: token })
        .send({
          urlName: "tech2",
        })
        .end((err, res) => {
          assert.equal(res.status, 200);
          assert.isTrue(res.body.success);
          done();
        });
    });

    it("DELETE /blogs/:blogUrlName/categories/:categoryUrlName - Trying to delete category without authorization", (done) => {
      chai
        .request(server)
        .delete("/blogs/blogtest/categories/tech2")
        .end((err, res) => {
          assert.equal(res.status, 401);
          done();
        });
    });

    it("DELETE /blogs/:blogUrlName/categories/:categoryUrlName - Delete category", (done) => {
      chai
        .request(server)
        .delete("/blogs/blogtest/categories/tech2")
        .set({ Authorization: token })
        .end((err, res) => {
          assert.equal(res.status, 200);
          assert.isTrue(res.body.success);
          done();
        });
    });

    after((done) => {
      chai
        .request(server)
        .delete("/blogs/blogtest")
        .set({ Authorization: token })
        .end((err, res) => {
          chai
            .request(server)
            .delete("/users/testUser2")
            .set({ Authorization: token })
            .end((err, res) => {
              if (res.status === 200) done();
            });
        });
    });
  });

  describe("Posts", () => {
    let token;
    before((done) => {
      chai
        .request(server)
        .post("/users/register")
        .send({
          email: "test.user837263@tst.example.co",
          username: "testUser",
          password: "1234@qwertdfg",
        })
        .end((err, res) => {
          chai
            .request(server)
            .post("/users/login")
            .send({
              username: "testUser",
              password: "1234@qwertdfg",
            })
            .end((err, res) => {
              token = res.body.token;

              chai
                .request(server)
                .post("/blogs")
                .set({ Authorization: token })
                .send({
                  urlName: "test-blog",
                  fullName: "The Test Blog",
                })
                .end((err, res) => {
                  chai
                    .request(server)
                    .post("/blogs/test-blog/tags")
                    .set({ Authorization: token })
                    .send({
                      urlName: "eg-tag",
                      fullName: "Example tag",
                    })
                    .end((err, res) => {
                      done();
                    });
                });
            });
        });
    });

    it("POST /blogs/:blogUrlName/posts/ - Trying to create post without authorization", (done) => {
      chai
        .request(server)
        .post("/blogs/test-blog/posts")
        .send({
          urlName: "learn-js",
          fullName: "Learn JavaScript!",
          tags: ["eg-tag"],
        })
        .end((err, res) => {
          assert.equal(res.status, 401);
          done();
        });
    });

    it("POST /blogs/:blogUrlName/posts/ - Create new post without fields", (done) => {
      chai
        .request(server)
        .post("/blogs/test-blog/posts")
        .set({ Authorization: token })
        .send({})
        .end((err, res) => {
          assert.equal(res.status, 400);
          assert.isFalse(res.body.success);
          done();
        });
    });

    it("POST /blogs/:blogUrlName/posts/ - Create new post", (done) => {
      chai
        .request(server)
        .post("/blogs/test-blog/posts")
        .set({ Authorization: token })
        .send({
          urlName: "learn-js",
          fullName: "Learn JavaScript!",
          tags: ["eg-tag"],
        })
        .end((err, res) => {
          assert.equal(res.status, 200);
          assert.isTrue(res.body.success);
          done();
        });
    });

    it("PUT /blogs/:blogUrlName/posts/:postUrlName - Edit post", (done) => {
      chai
        .request(server)
        .put("/blogs/test-blog/posts/learn-js")
        .set({ Authorization: token })
        .send({
          urlName: "learn-js-2",
        })
        .end((err, res) => {
          assert.equal(res.status, 200);
          assert.isTrue(res.body.success);
          done();
        });
    });

    it("GET /blogs/:blogUrlName/posts/:postUrlName - Read post", (done) => {
      chai
        .request(server)
        .get("/blogs/test-blog/posts/learn-js-2")
        .end((err, res) => {
          assert.equal(res.status, 200);
          assert.isTrue(res.body.success);
          assert.equal(res.body.data.urlName, "learn-js-2");
          assert.equal(res.body.data.fullName, "Learn JavaScript!");

          done();
        });
    });

    it("DELETE /blogs/:blogUrlName/posts/:postsUrlName - Delete post without authorization", (done) => {
      chai
        .request(server)
        .delete("/blogs/test-blog/posts/learn-js")
        .end((err, res) => {
          assert.equal(res.status, 401);
          done();
        });
    });

    it("DELETE /blogs/:blogUrlName/posts/:postsUrlName - Delete post", (done) => {
      chai
        .request(server)
        .delete("/blogs/test-blog/posts/learn-js-2")
        .set({ Authorization: token })
        .end((err, res) => {
          assert.equal(res.status, 200);
          done();
        });
    });

    after((done) => {
      chai
        .request(server)
        .delete("/blogs/test-blog/tags/eg-tag")
        .set({ Authorization: token })
        .end((err, res) => {
          chai
            .request(server)
            .delete("/blogs/test-blog")
            .set({ Authorization: token })
            .end((err, res) => {
              chai
                .request(server)
                .delete("/users/testUser")
                .set({ Authorization: token })
                .end((err, res) => {
                  if (res.status === 200) done();
                });
            });
        });
    });
  });
});
