﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Yu.Data.Repositories
{
    /// <summary>
    /// 所有数据实体的基类
    /// </summary>
    /// <typeparam name="TPrimaryKey">主键类型</typeparam>
    public abstract class IEntity<TPrimaryKey>
    {
        /// <summary>
        /// 设定ID为主键(默认就是id为主键,不加key注解也可以).设置自增.如有其他情况可以在fluentapi中设置.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public TPrimaryKey Id;
    }
}
