const nodemailer = require("nodemailer");

/**
 * Send an email
 *
 * @param {string} to - Recipient's email address
 * @param {string} subject - Subject of the email
 * @param {string} message - Text message to send
 */
function sendEmail(to, subject, message) {
  var transporter = nodemailer.createTransport({
    service: "gmail",
    auth: {
      user: process.env.EMAIL_USER,
      pass: process.env.EMAIL_PASS,
    },
  });

  var mailOptions = {
    from: process.env.EMAIL_USER,
    to: to,
    subject: subject,
    html: message,
  };

  transporter.sendMail(mailOptions, function (error, info) {
    if (error) {
      console.log(error);
    } else {
      console.log("Email sent: " + info.response);
    }
  });
}

module.exports.sendEmail = sendEmail;
