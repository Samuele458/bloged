/** class representing an HTTP bad request error  */
class BadRequestError extends Error {
  /**
   * Create a bad request error
   * @param {string} message - Error message
   */
  constructor(message) {
    super(message || "Bad request");

    //set status code
    this.statusCode = 400;
  }
}

/** class representing an HTTP not found error  */
class NotFoundError extends Error {
  /**
   * Create a not found error
   * @param {string} message - Error message
   */
  constructor(message) {
    super(message || "Not found");

    //set status code
    this.statusCode = 404;
  }
}

/** class representing an HTTP unauthorized error  */
class UnauthorizedError extends Error {
  /**
   * Create a unauthorized error
   * @param {string} message - Error message
   */
  constructor(message) {
    super(message || "Unauthorized");

    //set status code
    this.statusCode = 404;
  }
}

module.exports = { BadRequestError, NotFoundError, UnauthorizedError };
