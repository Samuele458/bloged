import "../../style/style.scss";

const Button = (props) => {
  const type = props.type || "button";
  const text = props.text || "Button";

  let btn;

  if (type === "button") {
    btn = <button className="p-4">{text}</button>;
  } else if (type === "link") {
    btn = <a href={props.link}>{text}</a>;
  } else if (type === "submit") {
    btn = <button type="submit">{text}</button>;
  }

  return <button className="ciao p-3">{text}</button>;
};

export default Button;
