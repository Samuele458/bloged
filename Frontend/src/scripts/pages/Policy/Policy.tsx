import React, { useEffect, useState } from "react";
import { marked } from "marked";

const parseMarkdown = (source: any) => {
  if (typeof source === "string") return { __html: marked(source) };
  else
    return (
      <div className="loading-box">
        <p className="loading-text">Loading...</p>
      </div>
    );
};

const Policy = (props: any) => {
  const [mdSource, setMdSource] = useState([]);

  const loadFile = (path: any) => {
    fetch(path)
      .then((response) => {
        return response.text();
      })
      .then((text) => {
        setMdSource(text as any);
      });
  };

  useEffect(() => {
    loadFile(props.filePath);
  });

  const content = parseMarkdown(mdSource);

  let contentBox;
  if (typeof (content as { __html: any }).__html === "string") {
    contentBox = (
      <div
        className="markdown-area policy-text"
        dangerouslySetInnerHTML={content as { __html: any }}
      ></div>
    );
  } else {
    contentBox = <div className="markdown-area policy-text">{content}</div>;
  }

  return <section className="policy-page">{contentBox}</section>;
};

export default Policy;
