import { SignInButton, SignUpButton, useAuth } from "@clerk/react";
import { Navigate, Route, Routes } from "react-router";
import { AppLayout } from "@/layouts/AppLayout";
import { AnalyticsPage } from "@/pages/AnalyticsPage";
import { DashboardPage } from "@/pages/DashboardPage";
import { SessionDetailPage } from "@/pages/SessionDetailPage";
import { SessionFormPage } from "@/pages/SessionFormPage";
import { SessionsListPage } from "@/pages/SessionsListPage";

function UnauthenticatedScreen() {
  return (
    <main className="flex min-h-screen items-center justify-center bg-background px-6">
      <div className="w-full max-w-sm rounded-2xl border border-border bg-card p-6 text-center shadow-sm">
        <h1 className="text-2xl font-semibold">Basketball Progress Tracker</h1>
        <p className="mt-2 text-sm text-muted-foreground">
          Sign in to log sessions, track streaks, and view analytics.
        </p>
        <div className="mt-6 flex items-center justify-center gap-3">
          <SignInButton />
          <SignUpButton />
        </div>
      </div>
    </main>
  );
}

function App() {
  const { isLoaded, isSignedIn } = useAuth();

  if (!isLoaded) {
    return null;
  }

  if (!isSignedIn) {
    return <UnauthenticatedScreen />;
  }

  return (
    <Routes>
      <Route element={<AppLayout />}>
        <Route index element={<DashboardPage />} />
        <Route path="sessions" element={<SessionsListPage />} />
        <Route path="sessions/new" element={<SessionFormPage mode="create" />} />
        <Route path="sessions/:id" element={<SessionDetailPage />} />
        <Route path="sessions/:id/edit" element={<SessionFormPage mode="edit" />} />
        <Route path="analytics" element={<AnalyticsPage />} />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Route>
    </Routes>
  );
}

export default App;
