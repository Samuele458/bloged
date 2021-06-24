require("dotenv").config();
const express = require("express");
const path = require("path");
const cors = require("cors");
const passport = require("passport");

const app = new express();

//MongoDB database
require("./config/database")(app);

//loading models
require("./models/user");
require("./models/blog");

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

const multer = require("multer");
const upload = multer({ dest: "uploads/" });
const s3Utils = require("./lib/s3Utils");
app.post("/images", upload.single("image"), async (req, res) => {
  const file = req.file;
  console.log(file);
  const result = await s3Utils.uploadToS3(file);
  console.log(result);
  res.send("okays");
});

app.use(require("./routes"));

const port = process.env.PORT || 3000;
app.listen(port, () => {
  console.log(`App listening on port ${port}`);
});

module.exports = app;
