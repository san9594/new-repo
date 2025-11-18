// React JS file with five different methods handling five different inputs
import React, { useState } from "react";

export default function ExampleMethods() {
  const [num, setNum] = useState(0);
  const [text, setText] = useState("");
  const [arr, setArr] = useState([1, 2, 3]);
  const [obj, setObj] = useState({ a: 1, b: 2 });
  const [flag, setFlag] = useState(true);

  // 1. Handle number input
  function handleNumberInput(value) {
    return value * 2;
  }

  // 2. Handle text input
  function handleTextInput(value) {
    return value.trim().toUpperCase();
  }

  // 3. Handle array input
  function handleArrayInput(value) {
    return value.join(", ");
  }

  // 4. Handle object input
  function handleObjectInput(value) {
    return Object.values(value);
  }

  // 5. Handle boolean input
  function handleBooleanInput(value) {
    return value ? "Boolean TRUE" : "Boolean FALSE";
  }

  return (
    <div style={{ padding: 20 }}>
      <h2>React JS Five Input Methods</h2>

      <p>Number Output: {handleNumberInput(num)}</p>
      <p>Text Output: {handleTextInput(text)}</p>
      <p>Array Output: {handleArrayInput(arr)}</p>
      <p>Object Output: {handleObjectInput(obj).join(" - ")}</p>
      <p>Boolean Output: {handleBooleanInput(flag)}</p>

      {/* Inputs */}
      <div style={{ marginTop: 20 }}>
        <input
          type="number"
          value={num}
          onChange={(e) => setNum(Number(e.target.value))}
          placeholder="Enter number"
        />

        <input
          type="text"
          value={text}
          onChange={(e) => setText(e.target.value)}
          placeholder="Enter text"
          style={{ marginLeft: 10 }}
        />

        <button style={{ marginLeft: 10 }} onClick={() => setFlag(!flag)}>
          Toggle Boolean
        </button>
      </div>
    </div>
  );
}
