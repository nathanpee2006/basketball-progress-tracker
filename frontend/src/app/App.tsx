import { useAuth } from "@clerk/react";
import { Navigate, Route, Routes } from "react-router";
import { AppLayout } from "@/layouts/AppLayout";
import { AnalyticsPage } from "@/features/analytics/AnalyticsPage";
import { DashboardPage } from "@/features/dashboard/DashboardPage";
import { SessionDetailPage } from "@/features/sessions-detail/SessionDetailPage";
import { SessionFormPage } from "@/features/create-edit-session-form/SessionFormPage";
import { SessionsPage } from "@/features/sessions-list/SessionsPage";
import { LeaderboardPage } from "@/features/leaderboard/LeaderboardPage";
import { AchievementsPage } from "@/features/achievements/AchievementsPage";
import { LandingPage } from "@/features/landing/LandingPage";

function App() {
  const { isLoaded, isSignedIn } = useAuth();

  if (!isLoaded) {
    return null;
  }

  if (!isSignedIn) {
    return <LandingPage />;
  }

  return (
    <Routes>
      <Route element={<AppLayout />}>
        <Route index element={<DashboardPage />} />
        <Route path="sessions" element={<SessionsPage />} />
        <Route path="sessions/new" element={<SessionFormPage mode="create" />} />
        <Route path="sessions/:id" element={<SessionDetailPage />} />
        <Route path="sessions/:id/edit" element={<SessionFormPage mode="edit" />} />
        <Route path="analytics" element={<AnalyticsPage />} />
        <Route path="leaderboard" element={<LeaderboardPage />} />
        <Route path="achievements" element={<AchievementsPage />} />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Route>
    </Routes>
  );
}

export default App;
