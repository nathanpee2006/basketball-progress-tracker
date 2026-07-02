import { useState, useEffect } from "react";
import { useAuth, SignInButton, SignUpButton, UserButton } from "@clerk/react";
import "./App.css";

function App() {
  const [data, setData] = useState(null);
  console.log(data);
  const { isSignedIn, getToken } = useAuth();

  useEffect(() => {
    if (!isSignedIn) return;

    const fetchSessions = async () => {
      const token = await getToken({ template: "jwt-basketball-progress-tracker" });  

      const timeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;
      
      const apiBaseUrl = "http://localhost:5134/api";
      const response = await fetch(`${apiBaseUrl}/sessions`, {
        headers: {
          Authorization: `Bearer ${token}`,
          "X-Time-Zone": timeZone
        }
      });

      if (!response.ok) throw new Error(`HTTP error: ${response.status}`);
      const result = await response.json();
      setData(result);
    };

    fetchSessions().catch(console.error);
  }, [isSignedIn]);  
         
    return (
    <>
      <header style={{ display: "flex", justifyContent: "space-between", alignItems: "center", padding: "1rem" }}>
        <h1>Basketball Progress Tracker</h1>
        {!isSignedIn ? (
          <div style={{ display: "flex", gap: "0.5rem" }}>
            <SignInButton />
            <SignUpButton />
          </div>
        ) : (
          <UserButton />
        )}
      </header>
      {data && <pre>{JSON.stringify(data, null, 2)}</pre>}
    </>
  ); 
}

export default App;
