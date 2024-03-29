﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 坦克大战_新_.Properties;

namespace 坦克大战_新_
{
    enum Tag
    { 
        MyTank,
        EnemyTank
    }

    class Bullet:Movething
    {
        public Tag Tag { set; get; }

        public bool IsDestroy { get; set; }

        public Bullet(int x, int y, int speed,Direction dir,Tag tag)
        {
            IsDestroy = false;
            this.X = x;
            this.Y = y;
            this.Speed = speed;
            BitmapDown = Resources.BulletDown;
            BitmapUp = Resources.BulletUp;
            BitmapRight = Resources.BulletRight;
            BitmapLeft = Resources.BulletLeft;
            this.Dir = dir;
            this.Tag = tag;

            this.X -= Width / 2;
            this.Y -= Height / 2;
        }

        public override void DrawSelf()
        {
            base.DrawSelf();
        }

        public override void Update()
        {
            MoveCheck();
            Move();
            base.Update();
        }

        private void MoveCheck()
        {

            # region 检查有无超出边界
            if (Dir == Direction.Up)
            {
                if (Y + Height / 2 + 3 < 0)
                {
                    IsDestroy = true;
                    return;
                }
            }
            else if (Dir == Direction.Down)
            {
                if (Y + Height / 2 - 3 > 450)
                {
                    IsDestroy = true;
                    return;
                }
            }
            else if (Dir == Direction.Left)
            {
                if (X + Width / 2 - 3 < 0)
                {
                    IsDestroy = true;
                    return;
                }
            }
            else if (Dir == Direction.Right)
            {
                if (X + Width / 2 + 3 > 450)
                {
                    IsDestroy = true;
                    return;
                }
            }
            #endregion 

            //检查有无与其他元素碰撞
            Rectangle rect = GetRectangle();

            rect.X = X + Width / 2 - 3;
            rect.Y = Y + Height / 2 - 3;
            rect.Width = 3;
            rect.Height = 3;

            //1.墙  2.钢体  3.坦克

            int xExplosion = this.X + Width / 2;
            int yExplosion= this.Y + Height / 2;


            NotMovething wall = null;
            if ((wall = GameObjectManager.IsCollidedWall(rect)) != null)
            {
                IsDestroy = true;
                GameObjectManager.DestroyWall(wall);
                GameObjectManager.CreateExplosion(xExplosion,yExplosion);
                SoundManager.PlayBlast();
                return;
            }

            if (GameObjectManager.IsCollidedSteel(rect) != null)
            {
                IsDestroy = true;
                SoundManager.PlayHit();
                GameObjectManager.CreateExplosion(xExplosion, yExplosion);
                return;
            }

            if (GameObjectManager.IsCollidedBoss(rect))
            {
                GameFramework.ChangeToGameOver();
                SoundManager.PlayBlast();
                return;
            }

            if (Tag == Tag.MyTank)
            {
                EnemyTank tank = null;
                if ((tank = GameObjectManager.IsCollidedEnemyTank(rect)) != null)
                {
                    IsDestroy = true;
                    GameObjectManager.DestroyTank(tank);
                    GameObjectManager.CreateExplosion(xExplosion, yExplosion);
                    SoundManager.PlayBlast();
                    return;
                }
            }else if (Tag == Tag.EnemyTank)
            {
                MyTank myTank = null;
                if ((myTank = GameObjectManager.IsCollidedMyTank(rect)) != null)
                {
                    IsDestroy = true;
                    GameObjectManager.CreateExplosion(xExplosion, yExplosion);
                    SoundManager.PlayBlast();
                    myTank.TakeDamage();

                    return;

                }
            }
        }

        private void Move()
        {

            switch (Dir)
            {
                case Direction.Up:
                    Y -= Speed;
                    break;
                case Direction.Down:
                    Y += Speed;
                    break;
                case Direction.Left:
                    X -= Speed;
                    break;
                case Direction.Right:
                    X += Speed;
                    break;
            }
        }
    }
}
