# SpaceInvaders

  1. Bugs reales

  - ok Un enemigo puede dar puntos dos veces (Game.cs:230-243). Si dos balas lo tocan en el mismo frame, el bucle no se
    detiene: suma Score++ dos veces y gasta las dos balas. Se arregla con un break; después de enemy.ShowCollision =
    true;.
  - ok La formación se va juntando con el tiempo (Game.cs:201-206). Cada enemigo se limita a la pantalla por separado.
    Cuando el de la orilla choca con el borde avanza menos que los demás, y en cada rebote se pierden unos píxeles de
    separación. Es mejor calcular el desplazamiento de todo el grupo y mover a todos igual.
  - ok La explosión se consume durante la pausa (Enemy.cs:110). El contador baja dentro de Draw(), que también se llama en
    pausa. Esa lógica de tiempo debería ir en un Update(), no en Draw.
  - ok Sin forma de reiniciar. En GameOver y End no hay entrada (Game.cs:112-114), así que solo puedes cerrar la ventana.
  - ok La bala sale un poco descentrada (Game.cs:160). Mide 6 px de ancho pero se resta 4. Debería ser - shot.Width / 2.

  2. Jugabilidad que falta

  - ok Player.Lifes existe pero no se usa: los enemigos no disparan, así que nunca pierdes vidas.
  - ?? Se puede disparar sin límite manteniendo Espacio. Conviene poner un tiempo de espera entre disparos o permitir una
    sola bala a la vez, como en el original.
  - ok Los enemigos bajan solo 2 px por rebote, así que casi nunca llegan a ti. Además no aceleran cuando quedan pocos.
  - ok Todos los enemigos valen 1 punto, sin importar su tipo.
  - 1/2  Falta guardar un récord de puntuación y pasar a un siguiente nivel en vez de terminar en "Nivel completado".

  3. Calidad de código

  - ok Código duplicado: Width, Height, Position, SetPositionX y SetPositionY se repiten en Player, Shot y Enemy. Una clase
    base Entity con una propiedad Rectangle Bounds quitaría esa repetición y simplificaría las colisiones.
  - ok Números fijos en el código: 100 (velocidad de los enemigos), 226, 300, 50, -120 y otros. Mejor pasarlos a constantes
    con nombre.
  - ok Textos centrados a mano (WidthWindow/2 - 80). Raylib.MeasureText los centraría correctamente.
  - ok Listas nuevas en cada frame: .Where(...).ToList() en MoveEnemys y en la limpieza de balas crea listas nuevas 60
    veces por segundo. ShotList.RemoveAll(...) evita esas asignaciones.
  - ok Setters públicos en todo, por ejemplo Score o GameStatus. Conviene private set donde nadie de fuera deba cambiarlos.
  - ok Nombres: MoveEnemys, SetEnemiList, UpdateShotsOutScreanOrImpact, enemyWith, fulPathIcon, Lifes e isEnemyRight (en
    minúscula). LoadGame() en realidad corre todo el juego, así que Run() describiría mejor lo que hace.
  - ok Advertencia de nulos: Player no se inicializa en el constructor y tienes Nullable activado.

  4. Proyecto

  - El README.md está vacío. Podría explicar los controles (S, ←/→, Espacio, P, C) y cómo compilar.
  - En el .csproj puedes reemplazar las tres entradas de recursos por una sola: <None Update="Resources\**"
    CopyToOutputDirectory="PreserveNewest" />.
  - No hay pruebas. Si separas la lógica (colisiones, movimiento) de Raylib, podrías probarla con xUnit.