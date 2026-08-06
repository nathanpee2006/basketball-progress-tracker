import { beforeEach, expect, test, vi } from "vitest";
import { page } from "vitest/browser";
import { render } from "vitest-browser-react";
import { MemoryRouter, Route, Routes } from "react-router";
import { SessionFormPage } from "../SessionFormPage";
import { userEvent } from "vitest/browser";
import { format } from "date-fns";
import { useSession } from "@/features/sessions-detail/useSession";

const today = new Date();
const expectedStoredDate = format(today, "yyyy-MM-dd");
const expectedAriaLabel = format(today, "EEEE, MMMM do, yyyy");

const paintMakes = 1;
const paintAttempts = 2;
const paintShotPercentage = (paintMakes / paintAttempts) * 100;
const midrangeMakes = 1;
const midrangeAttempts = 2;
const midrangeShotPercentage = (midrangeMakes / midrangeAttempts) * 100;
const threePointMakes = 1;
const threePointAttempts = 2;
const threePointShotPercentage = (threePointMakes / threePointAttempts) * 100;
const freeThrowMakes = 1;
const freeThrowAttempts = 2;
const freeThrowShotPercentage = (freeThrowMakes / freeThrowAttempts) * 100;
const overallMakes =
  paintMakes + midrangeMakes + threePointMakes + freeThrowMakes;
const overallAttempts =
  paintAttempts + midrangeAttempts + threePointAttempts + freeThrowAttempts;
const overallShotPercentage = (overallMakes / overallAttempts) * 100;

const createSessionMock = vi.fn(() => ({
  date: expectedStoredDate,
  paintMakes: paintMakes,
  paintAttempts: paintAttempts,
  midrangeMakes: midrangeMakes,
  midrangeAttempts: midrangeAttempts,
  threePointMakes: threePointMakes,
  threePointAttempts: threePointAttempts,
  freeThrowMakes: freeThrowMakes,
  freeThrowAttempts: freeThrowAttempts,
  drills: [],
}));

const newPaintMakes = 2;
const newMidrangeMakes = 2;
const newThreePointMakes = 2;
const newFreeThrowMakes = 2;

const updateSessionMock = vi.fn(() => ({
  date: expectedStoredDate,
  paintMakes: newPaintMakes,
  paintAttempts: paintAttempts,
  midrangeMakes: newMidrangeMakes,
  midrangeAttempts: midrangeAttempts,
  threePointMakes: newThreePointMakes,
  threePointAttempts: threePointAttempts,
  freeThrowMakes: newFreeThrowMakes,
  freeThrowAttempts: freeThrowAttempts,
  drills: [],
}));

vi.mock("../useCreateSession", () => ({
  useCreateSession: () => ({
    createSession: createSessionMock,
    isLoading: false,
    error: null,
  }),
}));

vi.mock("../useUpdateSession", () => ({
  useUpdateSession: () => ({
    updateSession: updateSessionMock,
    isLoading: false,
    error: null,
  }),
}));

vi.mock("../../sessions-detail/useSession", () => ({
  useSession: vi.fn(),
}));

const useSessionMock = vi.mocked(useSession);

beforeEach(() => {
  vi.clearAllMocks();
});

test("creates a session and navigates to sessions list", async () => {
  useSessionMock.mockReturnValue({
    session: null,
    isLoading: false,
    error: null,
  });

  render(
    <MemoryRouter initialEntries={["/sessions/new"]}>
      <Routes>
        <Route
          path="/sessions/new"
          element={<SessionFormPage mode="create" />}
        />
        <Route path="/sessions" element={<h1>Sessions</h1>} />
      </Routes>
    </MemoryRouter>,
  );

  await page.getByLabelText("Date").click();
  await page
    .getByRole("button", { name: new RegExp(expectedAriaLabel) })
    .click();
  await userEvent.keyboard("{Escape}");
  await page.getByLabelText("Paint Makes").fill("1");
  await page.getByLabelText("Paint Attempts").fill("2");
  await page.getByLabelText("Midrange Makes").fill("1");
  await page.getByLabelText("Midrange Attempts").fill("2");
  await page.getByLabelText("Three Point Makes").fill("1");
  await page.getByLabelText("Three Point Attempts").fill("2");
  await page.getByLabelText("Free Throw Makes").fill("1");
  await page.getByLabelText("Free Throw Attempts").fill("2");

  await page.getByRole("button", { name: "Save" }).click();

  await expect
    .poll(() => createSessionMock.mock.calls.length)
    .toBeGreaterThan(0);

  await expect
    .element(page.getByRole("heading", { name: "Sessions" }))
    .toBeVisible();

  expect(createSessionMock).toHaveBeenCalledWith({
    date: expectedStoredDate,
    paintMakes: paintMakes,
    paintAttempts: paintAttempts,
    midrangeMakes: midrangeMakes,
    midrangeAttempts: midrangeAttempts,
    threePointMakes: threePointMakes,
    threePointAttempts: threePointAttempts,
    freeThrowMakes: freeThrowMakes,
    freeThrowAttempts: freeThrowAttempts,
    drills: [],
  });
});

test("updates a session and navigates to session detail view", async () => {
  useSessionMock.mockReturnValue({
    session: {
      id: 1,
      date: expectedStoredDate,
      paintMakes: paintMakes,
      paintAttempts: paintAttempts,
      paintShotPercentage: paintShotPercentage,
      midrangeMakes: midrangeMakes,
      midrangeAttempts: midrangeAttempts,
      midrangeShotPercentage: midrangeShotPercentage,
      threePointMakes: threePointMakes,
      threePointAttempts: threePointAttempts,
      threePointShotPercentage: threePointShotPercentage,
      freeThrowMakes: freeThrowMakes,
      freeThrowAttempts: freeThrowAttempts,
      freeThrowShotPercentage: freeThrowShotPercentage,
      overallMakes: overallMakes,
      overallAttempts: overallAttempts,
      overallShotPercentage: overallShotPercentage,
      drills: [],
    },
    isLoading: false,
    error: null,
  });

  render(
    <MemoryRouter initialEntries={["/sessions/1/edit"]}>
      <Routes>
        <Route
          path="/sessions/:id/edit"
          element={<SessionFormPage mode="edit" />}
        />
        <Route
          path="/sessions/:id"
          element={
            <h1>
              {today.toLocaleDateString(undefined, {
                weekday: "long",
                year: "numeric",
                month: "long",
                day: "numeric",
              })}
            </h1>
          }
        />
      </Routes>
    </MemoryRouter>,
  );

  await expect
    .element(page.getByRole("heading", { name: "Edit Session" }))
    .toBeVisible();

  expect(useSessionMock).toHaveBeenLastCalledWith(1);

  await expect
    .element(page.getByLabelText("Paint Makes"))
    .toHaveValue(paintMakes);
  await expect
    .element(page.getByLabelText("Paint Attempts"))
    .toHaveValue(paintAttempts);
  await expect
    .element(page.getByLabelText("Midrange Makes"))
    .toHaveValue(midrangeMakes);
  await expect
    .element(page.getByLabelText("Midrange Attempts"))
    .toHaveValue(midrangeAttempts);
  await expect
    .element(page.getByLabelText("Three Point Makes"))
    .toHaveValue(threePointMakes);
  await expect
    .element(page.getByLabelText("Three Point Attempts"))
    .toHaveValue(threePointAttempts);
  await expect
    .element(page.getByLabelText("Free Throw Makes"))
    .toHaveValue(freeThrowMakes);
  await expect
    .element(page.getByLabelText("Free Throw Attempts"))
    .toHaveValue(freeThrowAttempts);

  await page.getByLabelText("Paint Makes").fill("2");
  await page.getByLabelText("Midrange Makes").fill("2");
  await page.getByLabelText("Three Point Makes").fill("2");
  await page.getByLabelText("Free Throw Makes").fill("2");

  await page.getByRole("button", { name: "Save" }).click();

  await expect
    .element(
      page.getByRole("heading", {
        name: today.toLocaleDateString(undefined, {
          weekday: "long",
          year: "numeric",
          month: "long",
          day: "numeric",
        }),
      }),
    )
    .toBeVisible();

  expect(updateSessionMock).toHaveBeenCalledTimes(1);

  expect(updateSessionMock).toHaveBeenCalledWith(1, {
    date: expectedStoredDate,
    paintMakes: newPaintMakes,
    paintAttempts: paintAttempts,
    midrangeMakes: newMidrangeMakes,
    midrangeAttempts: midrangeAttempts,
    threePointMakes: newThreePointMakes,
    threePointAttempts: threePointAttempts,
    freeThrowMakes: newFreeThrowMakes,
    freeThrowAttempts: freeThrowAttempts,
    drills: [],
  });
});

test("cancel button navigates back to /sessions", async () => {
  useSessionMock.mockReturnValue({
    session: null,
    isLoading: false,
    error: null,
  });

  render(
    <MemoryRouter initialEntries={["/sessions/new"]}>
      <Routes>
        <Route
          path="/sessions/new"
          element={<SessionFormPage mode="create" />}
        />
        <Route path="/sessions" element={<h1>Sessions</h1>} />
      </Routes>
    </MemoryRouter>,
  );

  await page.getByRole("button", { name: "Cancel" }).click();

  await expect
    .element(page.getByRole("heading", { name: "Sessions" }))
    .toBeVisible();

  expect(createSessionMock).not.toHaveBeenCalled();
});

test("shows SessionNotFound when editing a session that doesn't exist", async () => {
  useSessionMock.mockReturnValue({
    session: null,
    isLoading: false,
    error: null,
  });

  render(
    <MemoryRouter initialEntries={["/sessions/999/edit"]}>
      <Routes>
        <Route
          path="/sessions/:id/edit"
          element={<SessionFormPage mode="edit" />}
        />
        <Route path="/sessions" element={<h1>Sessions</h1>} />
      </Routes>
    </MemoryRouter>,
  );

  await expect
    .element(page.getByText(/session not found/i))
    .toBeInTheDocument();

  await expect
    .element(page.getByLabelText("Paint Makes"))
    .not.toBeInTheDocument();

  await page.getByRole("button", { name: /back to sessions/i }).click();

  await expect
    .element(page.getByRole("heading", { name: "Sessions" }))
    .toBeVisible();
});

test("blocks submission and shows an error when makes exceeds attempts", async () => {
  useSessionMock.mockReturnValue({
    session: null,
    isLoading: false,
    error: null,
  });

  render(
    <MemoryRouter initialEntries={["/sessions/new"]}>
      <Routes>
        <Route
          path="/sessions/new"
          element={<SessionFormPage mode="create" />}
        />
        <Route path="/sessions" element={<h1>Sessions</h1>} />
      </Routes>
    </MemoryRouter>,
  );

  // Invalid: 3 makes out of only 2 attempts.
  await page.getByLabelText("Paint Makes").fill("3");
  await page.getByLabelText("Paint Attempts").fill("2");
  await page.getByLabelText("Midrange Makes").fill("1");
  await page.getByLabelText("Midrange Attempts").fill("2");
  await page.getByLabelText("Three Point Makes").fill("1");
  await page.getByLabelText("Three Point Attempts").fill("2");
  await page.getByLabelText("Free Throw Makes").fill("1");
  await page.getByLabelText("Free Throw Attempts").fill("2");

  await page.getByRole("button", { name: "Save" }).click();

  await expect
    .element(page.getByText(/makes cannot exceed attempts/i))
    .toBeInTheDocument();

  expect(createSessionMock).not.toHaveBeenCalled();

  await expect
    .element(page.getByRole("heading", { name: "New Session" }))
    .toBeVisible();
});
