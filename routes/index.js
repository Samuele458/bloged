const router = require("express").Router();

router.use("/users", require("./users"));

router.use("/blogs", require("./blogs"));

// /blogs

module.exports = router;
