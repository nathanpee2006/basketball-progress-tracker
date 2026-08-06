import { render } from "vitest-browser-react";
import { page } from "vitest/browser";
import { beforeEach, expect, test, vi } from "vitest";
import { MemoryRouter } from "react-router";
import { SessionsPage } from "../SessionsPage";
import { format } from "date-fns/format";

const navigateMock = vi.fn();
vi.mock("react-router", async (importOriginal) => {
  const actual = await importOriginal<typeof import("react-router")>();
  return {
    ...actual,
    useNavigate: () => navigateMock,
  };
});

const toastSuccessMock = vi.fn();
const toastErrorMock = vi.fn();
vi.mock("sonner", () => ({
  toast: {
    success: (...args: unknown[]) => toastSuccessMock(...args),
    error: (...args: unknown[]) => toastErrorMock(...args),
  },
}));

const useSessionsMock = vi.fn();
vi.mock("../useSessions", () => ({
  useSessions: () => useSessionsMock(),
}));

const today = format(new Date(), "M/d/yyyy");

const mockSession = {
  id: 1,
  date: today,
  paintShotPercentage: 50,
  midrangeShotPercentage: 50,
  threePointShotPercentage: 50,
  freeThrowShotPercentage: 50,
  overallShotPercentage: 50,
};

function renderPage() {
  return render(
    <MemoryRouter>
      <SessionsPage />
    </MemoryRouter>,
  );
}

beforeEach(() => {
  vi.clearAllMocks();
});

test("shows loading skeleton while sessions are loading", async () => {
  useSessionsMock.mockReturnValue({
    sessions: [],
    isLoading: true,
    error: null,
    deleteSession: vi.fn(),
  });

  renderPage();

  await expect
    .element(page.getByLabelText("sessions-skeleton-list"))
    .toBeInTheDocument();
});

test("shows 'No sessions yet' when there are no sessions", async () => {
  useSessionsMock.mockReturnValue({
    sessions: [],
    isLoading: false,
    error: null,
    deleteSession: vi.fn(),
  });

  renderPage();

  await expect.element(page.getByText(/No sessions yet/i)).toBeInTheDocument();
});

test("displays session date and data when sessions exist", async () => {
  useSessionsMock.mockReturnValue({
    sessions: [mockSession],
    isLoading: false,
    error: null,
    deleteSession: vi.fn(),
  });

  renderPage();

  await expect.element(page.getByText(mockSession.date)).toBeInTheDocument();
  await expect
    .element(page.getByText(mockSession.paintShotPercentage.toString()))
    .toBeInTheDocument();
  await expect
    .element(page.getByText(mockSession.midrangeShotPercentage.toString()))
    .toBeInTheDocument();
  await expect
    .element(page.getByText(mockSession.threePointShotPercentage.toString()))
    .toBeInTheDocument();
  await expect
    .element(page.getByText(mockSession.freeThrowShotPercentage.toString()))
    .toBeInTheDocument();
  await expect
    .element(page.getByText(mockSession.overallShotPercentage.toString()))
    .toBeInTheDocument();
});

test("shows an error toast when fetching sessions fails", async () => {
  useSessionsMock.mockReturnValue({
    sessions: [],
    isLoading: false,
    error: new Error("Failed to load sessions"),
    deleteSession: vi.fn(),
  });

  renderPage();

  await expect.poll(() => toastErrorMock.mock.calls.length).toBeGreaterThan(0);
  expect(toastErrorMock).toHaveBeenCalledWith("Failed to load sessions");
});

test("navigates to /sessions/new when the create button is clicked", async () => {
  useSessionsMock.mockReturnValue({
    sessions: [],
    isLoading: false,
    error: null,
    deleteSession: vi.fn(),
  });

  renderPage();

  await page.getByRole("button", { name: /log session|create/i }).click();

  expect(navigateMock).toHaveBeenCalledWith("/sessions/new");
});

test("navigates to /sessions/:id when a session's view action is clicked", async () => {
  useSessionsMock.mockReturnValue({
    sessions: [mockSession],
    isLoading: false,
    error: null,
    deleteSession: vi.fn(),
  });

  renderPage();

  await page.getByRole("button", { name: "Session actions" }).click();
  await page.getByRole("menuitem", { name: /view/i }).click();

  expect(navigateMock).toHaveBeenCalledWith(`/sessions/${mockSession.id}`);
});

test("navigates to /sessions/:id/edit when a session's edit action is clicked", async () => {
  useSessionsMock.mockReturnValue({
    sessions: [mockSession],
    isLoading: false,
    error: null,
    deleteSession: vi.fn(),
  });

  renderPage();

  await page.getByRole("button", { name: "Session actions" }).click();
  await page.getByRole("menuitem", { name: /edit/i }).click();

  expect(navigateMock).toHaveBeenCalledWith(`/sessions/${mockSession.id}/edit`);
});

test("deletes a session and shows a success toast", async () => {
  const deleteSessionMock = vi.fn().mockResolvedValue(undefined);
  useSessionsMock.mockReturnValue({
    sessions: [mockSession],
    isLoading: false,
    error: null,
    deleteSession: deleteSessionMock,
  });

  renderPage();

  await page.getByRole("button", { name: "Session actions" }).click();
  await page.getByRole("menuitem", { name: /delete/i }).click();

  await expect.poll(() => deleteSessionMock.mock.calls.length).toBeGreaterThan(0);
  expect(deleteSessionMock).toHaveBeenCalledWith(mockSession.id);
  expect(toastSuccessMock).toHaveBeenCalledWith("Session deleted successfully");
});