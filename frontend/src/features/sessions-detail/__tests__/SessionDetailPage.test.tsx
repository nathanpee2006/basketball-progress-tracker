import { render } from "vitest-browser-react";
import { page } from "vitest/browser";
import { beforeEach, expect, test, vi } from "vitest";
import { MemoryRouter, Route, Routes } from "react-router";
import { SessionDetailPage } from "../SessionDetailPage";
import { format } from "date-fns/format";

const useSessionMock = vi.fn();
vi.mock("../useSession", () => ({
  useSession: (...args: unknown[]) => useSessionMock(...args),
}));

const today = format(new Date(), "EEEE, MMMM d, yyyy");

const mockSession = {
  id: 1,
  date: today,
  paintMakes: 1,
  paintAttempts: 2,
  paintShotPercentage: 50,
  midrangeMakes: 1,
  midrangeAttempts: 2,
  midrangeShotPercentage: 50,
  threePointMakes: 1,
  threePointAttempts: 2,
  threePointShotPercentage: 50,
  freeThrowMakes: 1,
  freeThrowAttempts: 2,
  freeThrowShotPercentage: 50,
  overallMakes: 4,
  overallAttempts: 8,
  overallShotPercentage: 50,
  drills: [{ name: "4x4", completionTimeInSeconds: 120 }],
};

function renderDetailPage(id: string | number = mockSession.id) {
  return render(
    <MemoryRouter initialEntries={[`/sessions/${id}`]}>
      <Routes>
        <Route path="/sessions/:id" element={<SessionDetailPage />} />
        <Route path="/sessions" element={<h1>Sessions</h1>} />
        <Route path="/sessions/:id/edit" element={<h1>Edit Session</h1>} />
      </Routes>
    </MemoryRouter>,
  );
}

beforeEach(() => {
  vi.clearAllMocks();
});

test("shows loading skeleton while the session is loading", async () => {
  useSessionMock.mockReturnValue({
    session: null,
    isLoading: true,
    error: null,
  });

  renderDetailPage();

  await expect
    .element(page.getByLabelText("session-detail-skeleton"))
    .toBeInTheDocument();
});

test("shows SessionNotFound when the session fails to load", async () => {
  useSessionMock.mockReturnValue({
    session: null,
    isLoading: false,
    error: new Error("Not found"),
  });

  renderDetailPage();

  await expect
    .element(page.getByText(/session not found/i))
    .toBeInTheDocument();

  await page.getByRole("button", { name: /back to sessions/i }).click();

  await expect
    .element(page.getByRole("heading", { name: "Sessions" }))
    .toBeVisible();
});

test("shows SessionNotFound when there is no error but also no session", async () => {
  useSessionMock.mockReturnValue({
    session: null,
    isLoading: false,
    error: null,
  });

  renderDetailPage();

  await expect
    .element(page.getByText(/session not found/i))
    .toBeInTheDocument();
});

test("displays the session's data once it has loaded", async () => {
  useSessionMock.mockReturnValue({
    session: mockSession,
    isLoading: false,
    error: null,
  });

  renderDetailPage();

  await expect.element(page.getByTestId("session-detail-header-date")).toHaveTextContent(today);

  await expect
    .element(page.getByTestId("stat-row-paint"))
    .toHaveTextContent(
      `${mockSession.paintMakes}/${mockSession.paintAttempts} · ${mockSession.paintShotPercentage}%`,
    );
  await expect
    .element(page.getByTestId("stat-row-midrange"))
    .toHaveTextContent(
      `${mockSession.midrangeMakes}/${mockSession.midrangeAttempts} · ${mockSession.midrangeShotPercentage}%`,
    );
  await expect
    .element(page.getByTestId("stat-row-three-point"))
    .toHaveTextContent(
      `${mockSession.threePointMakes}/${mockSession.threePointAttempts} · ${mockSession.threePointShotPercentage}%`,
    );
  await expect
    .element(page.getByTestId("stat-row-free-throw"))
    .toHaveTextContent(
      `${mockSession.freeThrowMakes}/${mockSession.freeThrowAttempts} · ${mockSession.freeThrowShotPercentage}%`,
    );
  await expect
    .element(page.getByTestId("stat-row-overall"))
    .toHaveTextContent(
      `${mockSession.overallMakes}/${mockSession.overallAttempts} · ${mockSession.overallShotPercentage}%`,
    );

  await expect
    .element(page.getByText(mockSession.drills[0].name))
    .toBeInTheDocument();
});

test("calls useSession with the numeric id from the route", async () => {
  useSessionMock.mockReturnValue({
    session: mockSession,
    isLoading: false,
    error: null,
  });

  renderDetailPage(mockSession.id);

  await expect.element(page.getByTestId("session-detail-header-date")).toHaveTextContent(today);

  expect(useSessionMock).toHaveBeenCalledWith(mockSession.id);
});

test("edit button navigates to /sessions/:id/edit", async () => {
  useSessionMock.mockReturnValue({
    session: mockSession,
    isLoading: false,
    error: null,
  });

  renderDetailPage();

  await page.getByRole("button", { name: /edit/i }).click();

  await expect
    .element(page.getByRole("heading", { name: "Edit Session" }))
    .toBeVisible();
});
