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
          assert.equal(res.status, 401);
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
});
