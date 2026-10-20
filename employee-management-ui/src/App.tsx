
import { useCallback, useEffect, useState } from "react";
import "./App.css";

interface Employee {
  id: number;
  name: string;
  department: string;
  salary: number;
  isActive: boolean;
}

interface AuthResponse {
  accessToken: string;
  expiresAtUtc: string;
  email: string;
  fullName: string;
}

const API_URL = import.meta.env.VITE_API_URL || "http://localhost:5265/api";

function App() {
  const [token, setToken] = useState(
    () => sessionStorage.getItem("accessToken") ?? ""
  );
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [userEmail, setUserEmail] = useState(
    () => sessionStorage.getItem("userEmail") ?? ""
  );
  const [fullName, setFullName] = useState(
    () => sessionStorage.getItem("fullName") ?? ""
  );

  const [employees, setEmployees] = useState<Employee[]>([]);
  const [name, setName] = useState("");
  const [department, setDepartment] = useState("");
  const [salary, setSalary] = useState("");

  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [loggingIn, setLoggingIn] = useState(false);
  const [error, setError] = useState("");

  const authenticatedFetch = useCallback(
    async (url: string, options: RequestInit = {}) => {
      return fetch(url, {
        ...options,
        headers: {
          ...options.headers,
          Authorization: `Bearer ${token}`
        }
      });
    },
    [token]
  );

  const loadEmployees = useCallback(async () => {
    if (!token) {
      setEmployees([]);
      setLoading(false);
      return;
    }

    setLoading(true);

    try {
      const response = await authenticatedFetch(
        `${API_URL}/employees`
      );

      if (response.status === 401) {
        sessionStorage.removeItem("accessToken");
        sessionStorage.removeItem("userEmail");
        sessionStorage.removeItem("fullName");
        setToken("");
        setUserEmail("");
        setFullName("");
        setEmployees([]);
        setError("Your session has expired. Please log in again.");
        return;
      }

      if (!response.ok) {
        throw new Error("Could not load employees.");
      }

      const data: Employee[] = await response.json();
      setEmployees(data);
      setError("");
    } catch {
      setError(
        "Unable to load employees. Check that the backend is running."
      );
    } finally {
      setLoading(false);
    }
  }, [token, authenticatedFetch]);

  useEffect(() => {
    if (token) {
      void loadEmployees();
    } else {
      setEmployees([]);
      setLoading(false);
    }
  }, [token, loadEmployees]);

  async function login(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError("");
    setLoggingIn(true);

    try {
      const response = await fetch(`${API_URL}/auth/login`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({
          email: email.trim(),
          password
        })
      });

      if (!response.ok) {
        throw new Error(
          response.status === 401
            ? "Incorrect email or password."
            : "Login failed. Please try again."
        );
      }

      const data: AuthResponse = await response.json();

      if (new Date(data.expiresAtUtc).getTime() <= Date.now()) {
        throw new Error("The server returned an expired token.");
      }

      sessionStorage.setItem("accessToken", data.accessToken);
      sessionStorage.setItem("userEmail", data.email);
      sessionStorage.setItem("fullName", data.fullName);

      setUserEmail(data.email);
      setFullName(data.fullName);
      setToken(data.accessToken);
      setPassword("");
      setEmail("");
      setError("");
    } catch (err) {
      setError(
        err instanceof Error
          ? err.message
          : "Unable to log in. Please try again."
      );
    } finally {
      setLoggingIn(false);
    }
  }

  function logout() {
    sessionStorage.removeItem("accessToken");
    sessionStorage.removeItem("userEmail");
    sessionStorage.removeItem("fullName");

    setToken("");
    setUserEmail("");
    setFullName("");
    setEmployees([]);
    setPassword("");
    setError("");
  }

  async function addEmployee(
    event: React.FormEvent<HTMLFormElement>
  ) {
    event.preventDefault();
    setError("");

    const parsedSalary = Number(salary);

    if (
      !name.trim() ||
      !department.trim() ||
      salary.trim() === "" ||
      !Number.isFinite(parsedSalary) ||
      parsedSalary < 0
    ) {
      setError(
        "Enter a valid name, department, and non-negative salary."
      );
      return;
    }

    setSaving(true);

    try {
      const response = await authenticatedFetch(
        `${API_URL}/employees`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json"
          },
          body: JSON.stringify({
            name: name.trim(),
            department: department.trim(),
            salary: parsedSalary
          })
        }
      );

      if (response.status === 401) {
        logout();
        setError("Your session has expired. Please log in again.");
        return;
      }

      if (!response.ok) {
        const result = await response.json().catch(() => null);
        throw new Error(
          result?.detail ||
            result?.message ||
            "Could not create employee."
        );
      }

      setName("");
      setDepartment("");
      setSalary("");

      await loadEmployees();
    } catch (err) {
      setError(
        err instanceof Error
          ? err.message
          : "Could not add employee. Please try again."
      );
    } finally {
      setSaving(false);
    }
  }

  if (!token) {
    return (
      <main className="container">
        <header className="page-header">
          <div>
            <h1>Employee Management</h1>
            <p>Sign in to manage employees.</p>
          </div>
        </header>

        <section className="panel">
          <h2>Login</h2>

          {error && (
            <p className="error" role="alert">
              {error}
            </p>
          )}

          <form className="employee-form" onSubmit={login}>
            <label>
              Email
              <input
                type="email"
                autoComplete="username"
                value={email}
                onChange={(event) => setEmail(event.target.value)}
                required
                placeholder="Enter your email"
              />
            </label>

            <label>
              Password
              <input
                type="password"
                autoComplete="current-password"
                value={password}
                onChange={(event) => setPassword(event.target.value)}
                required
                placeholder="Enter your password"
              />
            </label>

            <button disabled={loggingIn} type="submit">
              {loggingIn ? "Signing in..." : "Login"}
            </button>
          </form>
        </section>
      </main>
    );
  }

  if (loading) {
    return (
      <main className="container">
        <p>Loading employees...</p>
      </main>
    );
  }

  return (
    <main className="container">
      <header className="page-header">
        <div>
          <h1>Employee Management</h1>
          <p>Manage your employees in one place.</p>
          <p>
            Signed in as {fullName || userEmail || "User"}
          </p>
        </div>

        <button type="button" onClick={logout}>
          Logout
        </button>
      </header>

      {error && (
        <p className="error" role="alert">
          {error}
        </p>
      )}

      <section className="summary">
        <article className="card">
          <span>Total employees</span>
          <h2>{employees.length}</h2>
        </article>

        <article className="card">
          <span>Active employees</span>
          <h2>
            {employees.filter((employee) => employee.isActive).length}
          </h2>
        </article>
      </section>

      <section className="panel">
        <h2>Add employee</h2>

        <form className="employee-form" onSubmit={addEmployee}>
          <label>
            Employee name
            <input
              value={name}
              onChange={(event) => setName(event.target.value)}
              required
              minLength={2}
              maxLength={100}
              placeholder="Enter name"
            />
          </label>

          <label>
            Department
            <input
              value={department}
              onChange={(event) => setDepartment(event.target.value)}
              required
              minLength={2}
              maxLength={100}
              placeholder="Enter department"
            />
          </label>

          <label>
            Salary
            <input
              type="number"
              min="0"
              max="1000000000"
              step="0.01"
              value={salary}
              onChange={(event) => setSalary(event.target.value)}
              required
              placeholder="Enter salary"
            />
          </label>

          <button disabled={saving} type="submit">
            {saving ? "Saving..." : "Add employee"}
          </button>
        </form>
      </section>

      <section className="panel">
        <h2>Employee list</h2>

        {employees.length === 0 ? (
          <p>No employees found. Add your first employee above.</p>
        ) : (
          <div className="table-wrapper">
            <table>
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Name</th>
                  <th>Department</th>
                  <th>Salary</th>
                  <th>Status</th>
                </tr>
              </thead>

              <tbody>
                {employees.map((employee) => (
                  <tr key={employee.id}>
                    <td>{employee.id}</td>
                    <td>{employee.name}</td>
                    <td>{employee.department}</td>
                    <td>
                      {employee.salary.toLocaleString(undefined, {
                        minimumFractionDigits: 2,
                        maximumFractionDigits: 2
                      })}
                    </td>
                    <td>
                      <span
                        className={
                          employee.isActive
                            ? "status active"
                            : "status inactive"
                        }
                      >
                        {employee.isActive ? "Active" : "Inactive"}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </section>
    </main>
  );
}

export default App;
