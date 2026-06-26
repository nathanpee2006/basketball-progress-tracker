import { useState, useEffect } from "react";
import { Show, SignInButton, SignUpButton, UserButton } from "@clerk/react";
import "./App.css";

function App() {
  const [data, setData] = useState([]);
  console.log(data);

  useEffect(() => {
    const apiEndpoint = "http://localhost:5134/";

    fetch(apiEndpoint)
      .then((response) => {
        if (!response.ok) {
          throw new Error(`HTTP error! Status: ${response.status}`);
        }
        return response.json();
      })
      .then((data) => {
        setData(data);
      })
      .catch((error) => console.error(error));
  }, []);

  return (
    <>
      <header style={{ display: "flex", justifyContent: "space-between", alignItems: "center", padding: "1rem" }}>
        <h1>Basketball Progress Tracker</h1>
        <Show when="signed-out">
          <div style={{ display: "flex", gap: "0.5rem" }}>
            <SignInButton />
            <SignUpButton />
          </div>
        </Show>
        <Show when="signed-in">
          <UserButton />
        </Show>
      </header>
    </>
  );
}

export default App;
