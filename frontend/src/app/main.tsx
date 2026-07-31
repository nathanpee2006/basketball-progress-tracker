import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { BrowserRouter } from "react-router";
import "@/assets/index.css";
import { ClerkProvider } from "@clerk/react";
import App from "./App";
import { ThemeProvider } from "@/components/ui/theme-provider";
import { shadcn } from '@clerk/ui/themes'

const publishableKey = import.meta.env.VITE_CLERK_PUBLISHABLE_KEY;

if (!publishableKey) {
  throw new Error(
    "Missing VITE_CLERK_PUBLISHABLE_KEY — set it in .env file before starting the app",
  );
}

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <ClerkProvider appearance={{theme: shadcn}} publishableKey={publishableKey} afterSignOutUrl="/">
      <ThemeProvider defaultTheme="system" storageKey="vite-ui-theme">
        <BrowserRouter>
          <App />
        </BrowserRouter>
      </ThemeProvider>
    </ClerkProvider>
  </StrictMode>,
);
