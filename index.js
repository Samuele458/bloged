require("dotenv").config();
const express = require("express");
const path = require("path");
const cors = require("cors");
const passport = require("passport");

const app = new express();

//MongoDB database
require("./config/database");

//loading models
require("./models/user");

//fetch POST parameters
app.use(express.json());
app.use(express.urlencoded({ extended: true }));

//allow frontend to make requests to backend
app.use(cors());

//passport configuration
require("./config/passport");

//pass the global passport object into the configuration function
require("./config/passport")(passport);

//initialize the passport object on every request
app.use(passport.initialize());

app.use(require("./routes"));

app.get("*", (req, res) => {
  res.send(req.subdomains);
});

const port = process.env.PORT || 3000;
app.listen(port, () => {
  console.log(`App listening on port ${port}`);
});

module.exports = app;
