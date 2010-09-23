using IPDL;

public class App {
  public static int Main(string[] args) {
    var cli = new Cli();
    cli.Run(args);
    return cli.Succeeded ? 0 : 1;
  }
}
