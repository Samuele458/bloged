const passport = require("passport");

module.exports.requireRole = (role) => {
  return (req, res, next) => {
    let userRole = req.user.role;

    //check if user has access rights
    if (
      (userRole === "owner" && role === "owner") ||
      (userRole === "admin" && (role === "owner" || role === "admin")) ||
      (userROle === "subcriber" &&
        (role === "owner" || role === "admin" || role === "subcriber"))
    ) {
      //user can access
      next();
    } else {
      //user cannot access
      res.status(401).json({ msg: "Unathorized" });
    }
  };
};

module.exports.optionalAuthenticate = (req, res, next) => {
  const auth = req.header("Authorization");
  if (auth) {
    req.authenticated = true;
    return passport.authenticate("jwt", { session: false })(req, res, next);
  } else {
    req.authenticated = false;
    next();
  }
};
