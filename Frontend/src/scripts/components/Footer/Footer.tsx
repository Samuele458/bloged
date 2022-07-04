import React from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faDiscord,
  faFacebook,
  faInstagram,
  faLinkedin,
  faTelegram,
  faTwitter,
} from "@fortawesome/free-brands-svg-icons";
import { Link } from "react-router-dom";

export interface FooterProps {
  Title?: string;
  Logo?: string;
  Links: { To: string; Text: string }[];
  Socials: {
    Instagram?: string;
    Linkedin?: string;
    Twitter?: string;
    Opensea?: string;
    Discord?: string;
    Facebook?: string;
    Telegram?: string;
  };
}

const Footer = (props: FooterProps) => {
  return (
    <footer className="footer">
      <div className="footer-row">
        <img
          src="/build/images/logo-small.png"
          alt=""
          className="footer-image"
        />
      </div>
      <div className="footer-row">
        {props.Socials.Discord && (
          <a href={props.Socials.Discord} target="_blank">
            <FontAwesomeIcon icon={faDiscord} className="icon" />
          </a>
        )}

        {props.Socials.Facebook && (
          <a href={props.Socials.Facebook} target="_blank">
            <FontAwesomeIcon icon={faFacebook} className="icon" />
          </a>
        )}

        {props.Socials.Instagram && (
          <a href={props.Socials.Instagram} target="_blank">
            <FontAwesomeIcon icon={faInstagram} className="icon" />
          </a>
        )}

        {props.Socials.Telegram && (
          <a href={props.Socials.Telegram} target="_blank">
            <FontAwesomeIcon icon={faTelegram} className="icon" />
          </a>
        )}

        {props.Socials.Twitter && (
          <a href={props.Socials.Twitter} target="_blank">
            <FontAwesomeIcon icon={faTwitter} className="icon" />
          </a>
        )}

        {props.Socials.Linkedin && (
          <a href={props.Socials.Linkedin} target="_blank">
            <FontAwesomeIcon icon={faLinkedin} className="icon" />
          </a>
        )}

        {props.Socials.Opensea && (
          <a href={props.Socials.Opensea} target="_blank">
            <img src="/build/images/opensea-logo.svg" alt="" />
          </a>
        )}
      </div>
      <div className="footer-row">
        {props.Links.map((link, i) => (
          <Link to={link.To} className="footer-link" key={i}>
            {link.Text}
          </Link>
        ))}
      </div>
    </footer>
  );
};

export default Footer;
