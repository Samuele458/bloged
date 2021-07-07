let config = {};

//users roles. Each role has a "level", uset to determine permissions
//higher level -> more permissions
config.roles = [
  {
    role: "subscriber",
    level: 1,
  },
  {
    role: "admin",
    level: 3,
  },
  {
    role: "owner",
    level: 4,
  },
];

module.exports.config = config;
