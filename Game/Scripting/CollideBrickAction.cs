using System.Collections.Generic;
using Unit06.Game.Casting;
using Unit06.Game.Services;


namespace Unit06.Game.Scripting
{
    public class CollideBrickAction : Action
    {
        private AudioService audioService;
        private PhysicsService physicsService;
        
        public CollideBrickAction(PhysicsService physicsService, AudioService audioService)
        {
            this.physicsService = physicsService;
            this.audioService = audioService;
        }

        public void Execute(Cast cast, Script script, ActionCallback callback)
        {
            Ball ball = (Ball)cast.GetFirstActor(Constants.BALL_GROUP);
            Racket ship = (Racket)cast.GetFirstActor(Constants.RACKET_GROUP);
            List<Actor> bricks = cast.GetActors(Constants.BRICK_GROUP);
            Stats stats = (Stats)cast.GetFirstActor(Constants.STATS_GROUP);
            Sound overSound = new Sound(Constants.OVER_SOUND);
            
            foreach (Actor actor in bricks)
            {
                Brick brick = (Brick)actor;
                Body brickBody = brick.GetBody();
                Body ballBody = ball.GetBody();

                if (physicsService.HasCollided(brickBody, ballBody))
                {
                    Sound sound = new Sound(Constants.BOUNCE_SOUND);
                    audioService.PlaySound(sound);
                    int points = brick.GetPoints();
                    stats.AddPoints(points);
                    cast.RemoveActor(Constants.BRICK_GROUP, brick);
                }
            }
            foreach (Actor actor in bricks)
            {
                Brick brick = (Brick)actor;
                Body brickBody = brick.GetBody();
                Body ballBody = ball.GetBody();
                Point other = ballBody.GetPosition();

                if (physicsService.HasCollided(brickBody, ballBody) || other.GetY() <= Constants.FIELD_TOP )
                {
                    Point point = new Point(-100,100);
                    Point size = new Point(Constants.BALL_WIDTH, Constants.BALL_HEIGHT);
                    Point velocity = new Point(0, 0);
                    Body body = new Body(point, size, velocity);
                    ball.SetBody(body);
                }
            }

            foreach (Actor actor in bricks)
            {
                Brick brick = (Brick)actor;
                Body brickBody = brick.GetBody();
                Body shipBody = ship.GetBody();
                Point other = shipBody.GetPosition();

                if (physicsService.HasCollided(brickBody, shipBody))
                {
                    stats.RemoveLife();
                    if (stats.GetLives() > 0)
                    {
                        callback.OnNext(Constants.TRY_AGAIN);
                    }
                    else
                    {
                        callback.OnNext(Constants.GAME_OVER);
                        audioService.PlaySound(overSound);
                    }
                }
            }
        }
    }
}