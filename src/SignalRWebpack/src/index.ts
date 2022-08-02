import * as signalR from "@microsoft/signalr";
import "./css/main.css";

const divMessages: HTMLDivElement = document.querySelector("#divMessages");

const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5000/message")
    .build();

connection.on("SendMessage", async (message: string) => {
  const m = document.createElement("div");
  m.innerHTML = `<div>${message}</div>`;
  divMessages.appendChild(m);
  divMessages.scrollTop = divMessages.scrollHeight;
});

connection.start().catch((err) => document.write(err));