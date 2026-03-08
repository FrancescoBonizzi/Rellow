import { Application, Renderer, Text } from "pixi.js";

class LoadingThing {
  private readonly _loadingText: Text;
  private _app: Application<Renderer>;

  constructor(app: Application<Renderer>) {
    this._app = app;
    this._loadingText = new Text({
      text: "Mescolamento colori in corso...",
      style: {
        fontSize: 32,
        fontWeight: "bold",
        fill: { color: "#ffea87" },
        fontFamily: '"Press Start 2P", monospace',
      },
    });
    this._loadingText.anchor.set(0.5);
    this._loadingText.x = app.screen.width / 2;
    this._loadingText.y = app.screen.height / 2;
  }

  show(): void {
    // addChild è idempotente
    this._app.stage.addChild(this._loadingText);
  }

  hide(): void {
    // removeChild è idempotente
    this._app.stage.removeChild(this._loadingText);
  }
}

export default LoadingThing;
